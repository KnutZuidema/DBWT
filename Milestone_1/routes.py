import json

from flask import Flask, render_template, request, redirect, session, make_response
from passlib.hash import argon2

from database import SQLSession
from login import init_login


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
        route = request.form.get('route')
        user = sql_session.get_user(username)
        if not user:
            return render_template('bad_login.html')
        print('routes:', password, user.password)
        if not argon2.verify(password, user.password):
            return render_template('bad_login.html')
        response = make_response(redirect(route))
        response.set_cookie('username', username)
        response.set_cookie('hash', argon2.hash(user.password))
        return response

    return app
