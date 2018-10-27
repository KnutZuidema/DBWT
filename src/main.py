from routes import init_app

if __name__ == '__main__':
    app = init_app()
    app.run(host='localhost', port=7890, debug=True)
