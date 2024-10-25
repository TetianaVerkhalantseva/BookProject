$(document).ready(function() {
    var base = 'https://localhost:7291';
    $('#fetchData').click(function() {
    // $('#fetchData').click(function(id) {
        $.ajax({
            url: base + '/api/Book',
            // url: base + '/api/Book' +id,
            type: 'GET',
            dataType: 'json',
            success: function(data) {
                $('#dataContainer').text(JSON.stringify(data));
            },
            error: function(xhr, status, error) {
                console.error('Error fetching data', error);
            }
        });
    });
});