import json

from flask import Flask, render_template, request, redirect
from passlib.hash import argon2

from database import SQLSession
from login import init_login, LoginUser


def init_app():
    app = Flask('eMensa')
    app.config['SECRET_KEY'] = 'secret'
    sql_config = {
        "sqlalchemy.url": "sqlite:///eMensa.db"
    }
    login_manager = init_login(app, sql_config)

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
        session = SQLSession(sql_config)
        if request.method == 'GET':
            return render_template('login.html')
        username = request.form.get('username')
        password = request.form.get('password')
        route = request.form.get('route')
        user = session.get_user(username)
        if not user:
            return render_template('bad_login.html')
        if not argon2.verify(password, user.password):
            return render_template('bad_login.html')
        user = LoginUser()
        user.id = username
        login_manager.login_user(user, remember=True)
        return redirect(route)

    return app
