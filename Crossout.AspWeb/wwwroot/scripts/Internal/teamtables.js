
var matchData = [];

var matchId = $('.team-table').data('matchid');

$.ajax({
    url: '/data/match/' + matchId,
    dataType: 'json',
    success: function (json) {
        matchData = json;
        $('td.details-control > .loading').addClass('d-none');
        $('td.details-control > svg.plus').removeClass('d-none');
    }
});

var tables = [];
$('.team-table').each(function (i, e) {
    var id = $(e).data('tableid');
    var roundId = $(e).data('roundid');
    var table = $(e).DataTable({
        dom: '<tr>',
        order: [[8, 'desc']],
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

    tables.push({ id: id, roundId: roundId, table: table });
});

$('.team-table tbody').on('click', 'td.details-control', function () {
    var teamTable = $(this).closest('.team-table');
    var id = $(teamTable).data('tableid');
    var table = tables.find(x => x.id === id);
    var tr = $(this).closest('tr');
    var row = table.table.row(tr);

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
    var roundId = table.roundId;

    var div = $('<div/>');

    var html = '<div class="d-flex flex-row-reverse justify-content-between"><table class="table table-borderless" style="width:50%">' +
        '<thead>' +
        '<tr>' +
        '<th>Weapon</th>' +
        '<th>Damage</th>' +
        '</tr>' +
        '</thead>' +
        '<tbody>';

    var damages = [];
    if (roundId >= 0)
        damages = matchData.damageData.filter(x => x.userId === parseInt(rowData[9]) && x.roundId === roundId);
    else {
        damages = matchData.damageData.filter(x => x.userId === parseInt(rowData[9]));
        var combinedDamages = [];
        damages.forEach(function (e, i) {
            var combinedIndex = combinedDamages.indexOf(x => x.itemId === e.itemId);
            if (combinedIndex === -1) {
                combinedDamages.push(e);
            }
            else {
                combinedDamages[combinedIndex].damage += e.damage;
            }
        });
        damages = combinedDamages;
    }


    damages.forEach(function (e, i) {
        var image = ''
        if (e.imageExists)
            image = '<img class="mr-1" width="32"height="32" src="/img/items/' + e.itemId + '.png"></img>';
        var name = e.weaponDisplayName
        if (e.itemId !== 0)
            name = '<a href="/item/' + e.itemId + '">' + e.weaponDisplayName + '</a>';
        html += '<tr><td>' + image + name + '</td><td>' + e.damage + '</td></tr>';
    });
    html += '</tbody>';


    var medals = [];
    if (roundId >= 0)
        medals = matchData.medalData.filter(x => x.userId === parseInt(rowData[9]) && x.roundId === roundId);
    else {
        medals = matchData.medalData.filter(x => x.userId === parseInt(rowData[9]));
        var combinedMedals = [];
        medals.forEach(function (e, i) {
            var combinedIndex = combinedMedals.indexOf(x => x.medal === e.medal);
            if (combinedIndex === -1) {
                combinedMedals.push(e);
            }
            else {
                combinedMedals[combinedIndex].amount += e.amount;
            }
        });
        medals = combinedMedals;
    }

    html += '<ul class="list-unstyled" style="width:50%">';

    medals.forEach(function (e, i) {
        html += '<li>' + e.amount + ' ' + e.medal + '</li>';
    });

    html += '</ul></div>';

    div.html(html);

    return div;
}