
var matchData = [];

var matchId = $('.team-table').data('matchid');

$.ajax({
    url: '/data/match/' + matchId,
    dataType: 'json',
    success: function (json) {
        matchData = { damageData: json.damageData };
        $('td.details-control > .loading').addClass('d-none');
        $('td.details-control > svg.plus').removeClass('d-none');
    }
});

var tables = [];
$('.team-table').each(function (i, e) {
    var id = $(e).data('tableid');
    var table = $(e).DataTable({
        dom: '<tr>',
        order: [[7, 'desc']],
        searching: true,
        lengthChange: false,
        paging: false,
        info: false,
        autoWidth: false,
        columnDefs: [
            { width: '15%', targets: [1, 3, 7, 8] },
            { width: '12%', targets: [4, 5, 6, 2] },
            { orderable: false, width: '4%', targets: [0] },
            { visible: false, targets: [9] },
        ]
    });

    tables.push({ id: id, table: table });
});

$('.team-table tbody').on('click', 'td.details-control', function () {
    var teamTable = $(this).closest('.team-table');
    var id = $(teamTable).data('tableid');
    var table = tables.find(x => x.id === id).table;
    var tr = $(this).closest('tr');
    var row = table.row(tr);

    if (row.child.isShown()) {
        row.child.hide();
        tr.removeClass('shown');
        $(this).children('td.details-control > svg.minus').addClass('d-none');
        $(this).children('td.details-control > svg.plus').removeClass('d-none');
    }
    else {
        row.child(format(table, row.data())).show();
        tr.addClass('shown');
        $(this).children('td.details-control > svg.plus').addClass('d-none');
        $(this).children('td.details-control > svg.minus').removeClass('d-none');
    }
});

function format(table, rowData) {
    var div = $('<div/>');

    var html = '<table class="table table-borderless" style="width:50%">' +
        '<thead>' +
        '<tr>' +
        '<th>Weapon</th>' +
        '<th>Damage</th>' +
        '</tr>' +
        '</thead>' +
        '<tbody>';
    matchData.damageData.filter(x => x.userId === parseInt(rowData[9])).forEach(function (e, i) {
        var image = ''
        if (e.imageExists)
            image = '<img class="mr-1" width="32"height="32" src="/img/items/' + e.itemId + '.png"></img>';
        var name = e.weaponDisplayName
        if (e.itemId !== 0)
            name = '<a href="/item/' + e.itemId + '">' + e.weaponDisplayName + '</a>';
        html += '<tr><td>' + image + name + '</td><td>' + e.damage + '</td></tr>';
    });
    html += '</tbody>';
    div.html(html);

    return div;
}