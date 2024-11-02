$(document).ready(function() {
    var base = 'https://localhost:7291';
    var originalBookData = {};
    var originalAuthorData = {};
    var editingAuthorId = null;

    $('#addBookButton').click(function() {
        window.location.href = '/book/add';
    });

    $('#addBookForm').on('submit', function(event) {
        event.preventDefault();

        var newBook = {
            title: $('#title').val(),
            description: $('#description').val(),
            year: $('#year').val(),
            authorId: parseInt($('#authorId').val(), 10),
            categoryId: parseInt($('#categoryId').val(), 10),
            publisherId: parseInt($('#publisherId').val(), 10),
            languageId: parseInt($('#languageId').val(), 10)
        };

        $.ajax({
            url: `${base}/api/Book`,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(newBook),
            success: function() {
                window.location.href = '/';
            },
            error: function(xhr) {
                let errorMessage = 'An unexpected error occurred.';
                if (xhr.status === 409) {
                    errorMessage = 'A book with the same title already exists.';
                } else if (xhr.status === 400) {
                    errorMessage = xhr.responseText || 'Invalid input. Please check your data.';
                } else if (xhr.status === 500) {
                    errorMessage = 'An unexpected error occurred. Please try again later.';
                }
                alert(errorMessage);
                console.error('Error adding book:', xhr.responseText || error);
            }
        });
    });

    function fetchBooks() {
        $.ajax({
            url: `${base}/api/Book`,
            type: 'GET',
            dataType: 'json',
            success: function(data) {
                $('#dataContainer').empty();
                data.forEach(function(book) {
                    var bookCard = `
                        <div class="book-card" data-id="${book.id}">
                            <h2>${book.title || "Untitled"}</h2>
                            <p><strong>Author:</strong> ${book.author.firstName} ${book.author.lastName}</p>
                            <p><strong>Year:</strong> ${book.year}</p>
                            <p><strong>Category:</strong> ${book.category.name}</p>
                            <p><strong>Publisher:</strong> ${book.publisher.name}</p>
                            <p><strong>Language:</strong> ${book.language.name}</p>
                            <p><strong>Description:</strong> ${book.description}</p>
                            <button class="view-btn" data-id="${book.id}">View</button>
                            <button class="delete-btn" data-id="${book.id}">Delete</button>
                        </div>
                    `;
                    $('#dataContainer').append(bookCard);
                });

                $('.view-btn').click(function(event) {
                    event.stopPropagation();
                    var bookId = $(this).data('id');
                    window.location.href = `/book/${bookId}`;
                });

                $('.delete-btn').click(function(event) {
                    event.stopPropagation();
                    var bookId = $(this).data('id');
                    deleteBook(bookId);
                });
            },
            error: function(xhr) {
                let errorMessage = xhr.responseText || 'Error fetching data.';
                console.error('Error fetching data:', errorMessage);
            }
        });
    }

    function deleteBook(bookId) {
        if (!confirm('Are you sure you want to delete this book?')) {
            return;
        }

        $.ajax({
            url: `${base}/api/Book/${bookId}`,
            type: 'DELETE',
            success: function() {
                alert('Book successfully deleted!');
                fetchBooks();
            },
            error: function(xhr) {
                let errorMessage = xhr.responseText || 'Error deleting the book.';
                alert(errorMessage);
                console.error('Error deleting book:', errorMessage);
            }
        });
    }

    function fetchBookDetails(bookId) {
        $.ajax({
            url: `${base}/api/Book/${bookId}`,
            type: 'GET',
            dataType: 'json',
            success: function(book) {
                originalBookData = { ...book };
                renderBookDisplayMode(book);
            },
            error: function(xhr) {
                let errorMessage = xhr.responseText || 'Error fetching book details.';
                console.error('Error fetching book details:', errorMessage);
            }
        });
    }

    function renderBookDisplayMode(book) {
        var bookDetails = `
            <h1>${book.title || "Untitled"}</h1>
            <p><strong>Author:</strong> ${book.author.firstName} ${book.author.lastName}</p>
            <p><strong>Year:</strong> ${book.year}</p>
            <p><strong>Category:</strong> ${book.category.name}</p>
            <p><strong>Publisher:</strong> ${book.publisher.name}</p>
            <p><strong>Language:</strong> ${book.language.name}</p>
            <p><strong>Description:</strong> ${book.description}</p>
        `;
        $('#bookDetailContainer').html(bookDetails);
        $('#editButton').show();
        $('#saveButton, #cancelButton').hide();
        $('#backButton').show();
    }

    function renderBookEditMode(book) {
        var bookDetails = `
            <div>
                <label>Title:</label>
                <input type="text" id="title" value="${book.title || "Untitled"}">
            </div>
            <div>
                <label>Year:</label>
                <input type="number" id="year" value="${book.year}">
            </div>
            <div>
                <label>Author ID:</label>
                <input type="number" id="authorId" value="${book.author.id}">
            </div>
            <div>
                <label>Category ID:</label>
                <input type="number" id="categoryId" value="${book.category.id}">
            </div>
            <div>
                <label>Publisher ID:</label>
                <input type="number" id="publisherId" value="${book.publisher.id}">
            </div>
            <div>
                <label>Language ID:</label>
                <input type="number" id="languageId" value="${book.language.id}">
            </div>
            <div>
                <label>Description:</label>
                <textarea id="description">${book.description}</textarea>
            </div>
        `;
        $('#bookDetailContainer').html(bookDetails);
        $('#editButton').hide();
        $('#saveButton, #cancelButton').show();
        $('#backButton').hide();
    }

    $('#editButton').click(function() {
        renderBookEditMode(originalBookData);
        $('#editButton').hide();
        $('#saveButton, #cancelButton').show();
    });

    $('#cancelButton').click(function() {
        renderBookDisplayMode(originalBookData);
        $('#saveButton, #cancelButton').hide();
        $('#editButton').show();
        $('#backButton').show();
    });

    $('#saveButton').click(function() {
        var updatedBook = {
            id: originalBookData.id,
            title: $('#title').val(),
            description: $('#description').val(),
            year: parseInt($('#year').val(), 10),
            authorId: parseInt($('#authorId').val(), 10),
            categoryId: parseInt($('#categoryId').val(), 10),
            publisherId: parseInt($('#publisherId').val(), 10),
            languageId: parseInt($('#languageId').val(), 10)
        };

        $.ajax({
            url: `${base}/api/Book/${originalBookData.id}`,
            type: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify(updatedBook),
            success: function() {
                alert('Book updated successfully!');
                window.location.href = '/';  // Redirects to the index page after saving
            },
            error: function(xhr) {
                let errorMessage = xhr.responseText || 'Error updating the book.';
                alert(errorMessage);
                console.error('Error updating book:', errorMessage);
            }
        });
    });

    if ($('#bookDetailContainer').length) {
        var bookId = window.location.pathname.split('/').pop();
        fetchBookDetails(bookId);
        
        $('#backButton').click(function() {
            window.location.href = '/';  // Redirects to the index page when back button is clicked
        });
    } else {
        fetchBooks();
    }

    // Author-related functions

    // Show the add author form
    $('#addAuthorButton').click(function() {
        $('#addAuthorFormContainer').show();
        $('#addAuthorButton').hide();
    });

    // Hide the add author form and reset it
    $('#cancelAddAuthor').click(function() {
        $('#addAuthorFormContainer').hide();
        $('#addAuthorButton').show();
        $('#addAuthorForm')[0].reset();
    });

    // Submit the add author form
    $('#addAuthorForm').on('submit', function(event) {
        event.preventDefault();

        var newAuthor = {
            firstName: $('#firstName').val(),
            lastName: $('#lastName').val()
        };

        $.ajax({
            url: `${base}/api/Author`,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(newAuthor),
            success: function() {
                alert('Author added successfully!');
                $('#addAuthorFormContainer').hide();
                $('#addAuthorButton').show();
                $('#addAuthorForm')[0].reset();
                fetchAuthors(); // Refresh the authors list
            },
            error: function(xhr) {
                let errorMessage = 'An unexpected error occurred.';
                if (xhr.status === 409) {
                    errorMessage = 'An author with the same name already exists.';
                } else if (xhr.status === 400) {
                    errorMessage = xhr.responseText || 'Invalid input. Please check your data.';
                } else if (xhr.status === 500) {
                    errorMessage = 'An unexpected error occurred. Please try again later.';
                }
                alert(errorMessage);
                console.error('Error adding author:', xhr.responseText || error);
            }
        });
    });

    // Open the edit modal with overlay
    function openEditModal(author) {
        $('#modalOverlay').show();
        $('#editAuthorModal').show();
        $('#editFirstName').val(author.firstName);
        $('#editLastName').val(author.lastName);
        editingAuthorId = author.id;
    }

    // Close the edit modal and hide overlay
    function closeEditModal() {
        $('#modalOverlay').hide();
        $('#editAuthorModal').hide();
        $('#editAuthorForm')[0].reset();
        editingAuthorId = null;
    }

    // Fetch authors and populate the container
    function fetchAuthors() {
        $.ajax({
            url: `${base}/api/Author`,
            type: 'GET',
            dataType: 'json',
            success: function(data) {
                $('#authorDataContainer').empty();
                data.forEach(function(author) {
                    var authorCard = `
                        <div class="author-card" data-id="${author.id}">
                            <h2>${author.firstName} ${author.lastName}</h2>
                            <button class="edit-author-btn" data-id="${author.id}">Edit</button>
                            <button class="delete-author-btn" data-id="${author.id}">Delete</button>
                        </div>
                    `;
                    $('#authorDataContainer').append(authorCard);
                });

                $('.edit-author-btn').click(function(event) {
                    event.stopPropagation();
                    var authorId = $(this).data('id');
                    fetchAuthorDetails(authorId); // Fetch and open modal for editing
                });

                $('.delete-author-btn').click(function(event) {
                    event.stopPropagation();
                    var authorId = $(this).data('id');
                    deleteAuthor(authorId);
                });
            },
            error: function(xhr) {
                let errorMessage = xhr.responseText || 'Error fetching authors.';
                console.error('Error fetching authors:', errorMessage);
            }
        });
    }

    // Fetch a single author's details and open edit modal
    function fetchAuthorDetails(authorId) {
        $.ajax({
            url: `${base}/api/Author/${authorId}`,
            type: 'GET',
            dataType: 'json',
            success: function(author) {
                openEditModal(author);
            },
            error: function(xhr) {
                let errorMessage = xhr.responseText || 'Error fetching author details.';
                console.error('Error fetching author details:', errorMessage);
            }
        });
    }

    // Submit edit author form
    $('#editAuthorForm').on('submit', function(event) {
        event.preventDefault();

        var updatedAuthor = {
            id: editingAuthorId,
            firstName: $('#editFirstName').val(),
            lastName: $('#editLastName').val()
        };

        $.ajax({
            url: `${base}/api/Author/${editingAuthorId}`,
            type: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify(updatedAuthor),
            success: function() {
                alert('Author updated successfully!');
                closeEditModal();
                fetchAuthors(); // Refresh the authors list
            },
            error: function(xhr) {
                let errorMessage = xhr.responseText || 'Error updating author.';
                alert(errorMessage);
                console.error('Error updating author:', errorMessage);
            }
        });
    });

    // Close modal on cancel or close button
    $('#cancelEditAuthor, .close').click(function() {
        closeEditModal();
    });

    // Delete an author
    function deleteAuthor(authorId) {
        if (!confirm('Are you sure you want to delete this author?')) {
            return;
        }

        $.ajax({
            url: `${base}/api/Author/${authorId}`,
            type: 'DELETE',
            success: function() {
                alert('Author successfully deleted!');
                fetchAuthors(); // Refresh the authors list
            },
            error: function(xhr) {
                let errorMessage = xhr.responseText || 'Error deleting the author.';
                alert(errorMessage);
                console.error('Error deleting author:', errorMessage);
            }
        });
    }

    // Call fetchAuthors on page load
    if ($('#authorDataContainer').length) {
        fetchAuthors();
    }

});
