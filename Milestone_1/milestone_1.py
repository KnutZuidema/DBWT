from flask import Flask, render_template
import json

app = Flask(__name__)


@app.route('/')
def index():
    return render_template('index.html')


@app.route('/products')
def products():
    with open('data/products.json') as file:
        products = json.load(file)['products']
    return render_template('products.html', products=products)


@app.route('/products/<int:id>')
def details(id: int):
    with open('data/products.json') as file:
        products = json.load(file)['products']
    return render_template('details.html', product=products[id])


if __name__ == '__main__':
    app.run(host='localhost', port=7890, debug=True)
