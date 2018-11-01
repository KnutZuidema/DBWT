import json

from flask import Flask, render_template, request, redirect, make_response
from passlib.hash import argon2

from .database import SQLSession, User
from .login import init_login


def init_app():
    app = Flask('eMensa')
    app.config['SECRET_KEY'] = 'secret'
    sql_config = {
        "sqlalchemy.url": "sqlite:///eMensa.db"
    }
    init_login(app, sql_config)

    @app.route('/')
    def index():
        return render_template('index.html')

    @app.route('/products')
    def products():
        with open('data/products.json', encoding='utf-8') as file:
            products = json.load(file)['products']
        return render_template('products.html', products=products)

    @app.route('/products/<int:id>')
    def details(id: int):
        with open('data/products.json', encoding='utf-8') as file:
            products = json.load(file)['products']
        return render_template('details.html', product=products[id])

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
        user = sql_session.get_user(username)
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
        if not 4 < len(username) < 30:
            return "Benutzername muss zwischen 4 und 30 Zeichen lang sein"
        if len(password) < 8:
            return "Passwort muss mindestens 8 Zeichen lang sein"
        route = request.form.get('route')
        sql_session = SQLSession(sql_config)
        user = sql_session.get_user(username)
        if user:
            return 'Benutzername existiert bereits'
        hash = argon2.hash(password)
        sql_session.add(User(name=username, password=hash))
        sql_session.commit()
        response = make_response(redirect(route))
        response.set_cookie('username', username)
        response.set_cookie('hash', argon2.hash(hash))
        return response

    return app
