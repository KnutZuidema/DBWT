import re

from flask import Flask, request
from passlib.hash import argon2

from database import SQLSession


class User:
    def __init__(self, authenticated: bool):
        self.authenticated = authenticated


def init_login(app: Flask, config: dict):
    @app.context_processor
    def inject_current_user():
        username = request.cookies.get('username')
        hash = request.cookies.get('hash')
        authenticated = False
        if username and is_argon2_hash(hash):
            sql_session = SQLSession(config)
            user = sql_session.get_user(username)
            print('login:', user.password, hash)
            authenticated = argon2.verify(user.password, hash)
        print('login:', authenticated)
        current_user = User(authenticated)
        return {'current_user': current_user}


def is_argon2_hash(hash: str) -> bool:
    if not hash:
        return False
    return bool(re.match(r'\$argon2(.*)\$v=(.*)\$m=(.*),t=(.*),p=(.*)\$(.*)\$(.*)', hash))
