from flask import Flask, render_template, jsonify


app = Flask(__name__)
app.secret_key = "qweasd" # Make safer


@app.route("/")
def index():
    return render_template('index.html')

@app.route('/book/add')
def add_book():
    return render_template('add_book.html')

@app.route('/authors')
def authors():
    return render_template('authors.html')


@app.route('/book/<int:book_id>')
def book_detail(book_id):
    return render_template('book_detail.html', book_id=book_id)



if __name__ == '__main__':
    app.run(debug = True)
