from flask import Flask
from flask_login import LoginManager
from passlib.hash import argon2

from database import SQLSession


class LoginUser:
    def __init__(self):
        self.id = None
        self.is_authenticated = False
        self.is_active = True
        self.is_anonymous = False

    def get_id(self):
        return self.id


def init_login(app: Flask, config: dict):
    login_manager = LoginManager()
    login_manager.init_app(app)

    @login_manager.user_loader
    def user_loader(username: str):
        session = SQLSession(config)
        if not session.get_user(username):
            return
        user = LoginUser()
        user.id = username
        return user

    @login_manager.request_loader
    def request_loader(request):
        username = request.form.get('username')
        session = SQLSession(config)
        db_user = session.get_user(username)
        if not db_user:
            return
        user = LoginUser()
        user.id = username
        password = request.form.get('password')
        user.is_authenticated = argon2.verify(password, db_user.password)
        return user

    return login_manager
