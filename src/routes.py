import json
import uuid
from datetime import date
from os.path import join
from pathlib import Path

from flask import Flask, render_template, request, redirect, make_response
from passlib.hash import argon2

from database import SQLSession, User
from login import init_login


def init_app():
    app = Flask('eMensa')
    app.config['SECRET_KEY'] = 'secret'
    sql_config = {
        "sqlalchemy.url": "mysql+pymysql://root:password@localhost/emensa"
    }
    init_login(app, sql_config)

    @app.route('/')
    def index():
        return render_template('index.html')

    @app.route('/products')
    def products():
        sql_session = SQLSession(sql_config)
        products = sql_session.execute('select m.description, i.file_path, m.id, m.available '
                                       'from image i join meal m '
                                       'on(m.id, i.id) in (select * from meal_image_relation)')
        return render_template('products.html', products=products)

    @app.route('/products/<int:id>')
    def details(id: int):
        sql_session = SQLSession(sql_config)
        product = sql_session.execute('select m.description, i.file_path '
                                      'from image i join meal m '
                                      'on(m.id, i.id) in (select * from meal_image_relation) '
                                      'where m.id = %s', (id,))[0]
        return render_template('details.html', product=product)

    @app.route('/impressum')
    def impressum():
        return render_template('impressum.html')

    @app.route('/login', methods=['GET', 'POST'])
    def login():
        sql_session = SQLSession(sql_config)
        if request.method == 'GET':
            return render_template('login.html')
        username = request.form.get('username')
        password = request.form.get('password')
        route = request.form.get('route', '/')
        user = sql_session.get_user_by_name(username)
        if not user:
            return render_template('bad_login.html')
        if not argon2.verify(password, user.password):
            return render_template('bad_login.html')
        response = make_response(redirect(route))
        response.set_cookie('username', username)
        response.set_cookie('hash', argon2.hash(user.password))
        return response

    @app.route('/logout')
    def logout():
        route = request.args.get('route')
        response = make_response(redirect(route))
        response.set_cookie('username', '', expires=0)
        response.set_cookie('hash', '', expires=0)
        return response

    @app.route('/register', methods=['GET', 'POST'])
    def register():
        if request.method == 'GET':
            return render_template('register.html')
        username = request.form.get('username')
        password = request.form.get('password')
        if not 4 <= len(username) <= 30:
            return "Benutzername muss zwischen 4 und 30 Zeichen lang sein"
        if len(password) < 8:
            return "Passwort muss mindestens 8 Zeichen lang sein"
        route = request.form.get('route')
        sql_session = SQLSession(sql_config)
        user = sql_session.get_user_by_name(username)
        if user:
            return 'Benutzername existiert bereits'
        hash = argon2.hash(password)
        sql_session.add(User(name=username, password=hash))
        sql_session.commit()
        response = make_response(redirect(route))
        response.set_cookie('username', username)
        response.set_cookie('hash', argon2.hash(hash))
        return response

    @app.route('/products/<int:id>/edit', methods=['GET', 'POST'])
    def edit_product(id: int):
        with open('data/products.json', encoding='utf-8') as file:
            products = json.load(file)['products']
        if request.method == 'GET':
            return render_template('edit_product.html', product=products[id])
        description = request.form.get('description')
        ingredient = request.form.get('ingredient')
        amount = request.form.get('amount')
        file = request.files.get('image')
        if description:
            products[id]['description'] = description
            response = render_template('edit_product.html', product=products[id])
        elif ingredient and amount:
            products[id]['ingredients'][ingredient] = amount
            response = render_template('edit_product.html', product=products[id], ingredients=True)
        elif file:
            filename = f'{uuid.uuid4()}.image'
            pathname = f'{date.today().year}/{date.today().month}/{date.today().day}'
            products[id]['image'] = f'{pathname}/{filename}'
            Path(join('static', 'assets', pathname)).mkdir(parents=True, exist_ok=True)
            file.save(join('static', 'assets', pathname, filename))
            response = render_template('edit_product.html', product=products[id], image=True)
        else:
            response = 'Bad Request'
        with open('data/products.json', 'w') as file:
            json.dump({'products': products}, file)
        return response

    @app.route('/ingredients')
    def ingredients():
        sql_session = SQLSession(sql_config)
        ingredients = sql_session.execute('select name, organic, vegetarian, vegan, gluten_free '
                                          'from ingredient')
        return render_template('ingredients.html', ingredients=ingredients)

    return app
