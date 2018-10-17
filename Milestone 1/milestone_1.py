from flask import Flask, render_template

app = Flask(__name__)


@app.route('/')
def index():
    return render_template('index.html')


@app.route('/products')
def products():
    return render_template('products.html')


@app.route('/products/<name>')
def details(name: str):
    return render_template('details.html', name=name)


if __name__ == '__main__':
    app.run(host='localhost', port=7890, debug=True)
