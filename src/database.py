from typing import Any

from sqlalchemy import engine_from_config, Column, Integer, String, Float, Boolean, ForeignKey
from sqlalchemy.engine.base import Engine
from sqlalchemy.ext.declarative import declarative_base
from sqlalchemy.orm import sessionmaker, Session


class SQLSession:

    def __init__(self, config: dict):
        self.engine: Engine = engine_from_config(config)
        self.session: Session = sessionmaker(bind=self.engine)()
        Model.metadata.create_all(self.engine)

    def close(self):
        self.session.close()

    def add(self, instance: Any, _warn: bool = True):
        self.session.add(instance, _warn)

    def commit(self):
        self.session.commit()

    def get_user_by_name(self, name: str):
        return self.session.query(User).filter(User.name == name).first()

    def get_user_by_id(self, id: int):
        return self.session.query(User).filter(User.id == id).first()

    def execute(self, statement: str, insert: tuple = tuple()):
        connection = self.engine.connect()
        query = connection.execute(statement, insert)
        keys = query.keys()
        values = query.fetchall()
        connection.close()
        query = []
        for value in values:
            query.append(dict(zip(keys, value)))
        return query


Model = declarative_base()


class User(Model):
    __tablename__ = 'users'

    id = Column(Integer, autoincrement=True, primary_key=True)
    name = Column(String(30), unique=True, nullable=False)
    password = Column(String(76), nullable=False)


class Product(Model):
    __tablename__ = 'products'

    id = Column(Integer, autoincrement=True, primary_key=True)
    name = Column(String(32))
    description = Column(String(1024))
    image_path = Column(String(256))
    price = Column(Float)
    available = Column(Boolean)


class Ingredient(Model):
    __tablename__ = 'ingredients'

    id = Column(Integer, autoincrement=True, primary_key=True)
    name = Column(String(32))
    amount = Column(Float)
    product_id = Column(Integer, ForeignKey('products.id'))
