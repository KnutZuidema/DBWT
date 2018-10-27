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

    def get_user(self, name: str):
        return self.session.query(User).filter(User.name == name).first()


Model = declarative_base()


class User(Model):
    __tablename__ = 'users'

    id = Column(Integer, autoincrement=True, primary_key=True)
    name = Column(String, unique=True, nullable=False)
    password = Column(String, nullable=False)


class Product(Model):
    __tablename__ = 'products'

    id = Column(Integer, autoincrement=True, primary_key=True)
    name = Column(String)
    description = Column(String)
    image_path = Column(String)
    price = Column(Float)
    available = Column(Boolean)


class Ingredient(Model):
    __tablename__ = 'ingredients'

    id = Column(Integer, autoincrement=True, primary_key=True)
    name = Column(String)
    amount = Column(Float)
    product_id = Column(Integer, ForeignKey('products.id'))
