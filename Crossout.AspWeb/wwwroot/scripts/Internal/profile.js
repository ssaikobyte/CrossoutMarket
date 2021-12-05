class Stats {
    constructor() {
        this.games = 0;
        this.rounds = 0;
        this.wins = 0;
        this.kills = 0;
        this.assists = 0;
        this.deaths = 0;
        this.drone_kills = 0;
        this.mvp = 0;
        this.medal = 0;
        this.score = 0;
        this.damage = 0;
        this.damage_rec = 0;
        this.time = 0;
    }

    add_game(game) {
        let mvp = 0;
        let medal = 0;

        if (game["medal_list"] != null) {
            game["medal_list"].split(',').forEach(x => {
                let medals = x.split(':');
                medal += parseInt(medals[1]);

                if (medals[0] == 'PvpMvpWin')
                    mvp += parseInt(medals[1]);
            });
        }

        this.games += 1;
        this.rounds += game["rounds"];
        this.wins += game["result"] === "Win" ? 1 : 0;
        this.kills += game["kills"];
        this.assists += game["assists"];
        this.deaths += game["deaths"];
        this.drone_kills += game["drone_kills"];
        this.mvp += mvp;
        this.medal += medal;
        this.score += game["score"];
        this.damage += game["damage"];
        this.damage_rec += game["damage_rec"];
        this.time += game["time_spent"];
    }

    get kda() {
        return ((this.kills + this.assists) / this.rounds).toFixed(2);
    }

    get win_rate() {
        return (((this.wins) / this.games) * 100).toFixed(1) + '%';
    }

    get mvp_rate() {
        return (((this.mvp) / this.games) * 100).toFixed(1) + '%';
    }

    get avg_kills() {
        return (this.kills / this.rounds).toFixed(2);
    }

    get avg_assists() {
        return (this.assists / this.rounds).toFixed(2);
    }

    get avg_damage() {
        return (this.damage / this.rounds).toFixed(0);
    }

    get avg_damage_rec() {
        return (this.damage_rec / this.rounds).toFixed(0);
    }

    get avg_score() {
        return (this.score / this.rounds).toFixed(0);
    }

    get time_spent() {
        let total_seconds = this.time;
        let days = Math.floor(total_seconds / 86400);
        total_seconds %= 86400;
        let hours = Math.floor(total_seconds / 3600);
        total_seconds %= 3600;
        let minutes = Math.floor(total_seconds / 60);
        total_seconds %= 60;

        if (days > 0)
            return days + 'd ' + hours + 'h';
        if (hours > 0)
            return hours + 'h ' + minutes + 'm';
        if (minutes > 0)
            return minutes + 'm ' + total_seconds + 's';
    }
};

var Uid = window.location.pathname.split("/").pop();
var match_history = [];
var active_classification = null;
var active_game_type = null;
var damage_list = [];
var dmg_rec_list = [];
var kd_list = [];
var score_list = [];
var filter_delay;

const start_date = datepicker('#start_date', { id: 1, dateSelected: moment().subtract(6, 'days').toDate() });
const end_date = datepicker('#end_date', { id: 1, dateSelected: moment().toDate() });

Highcharts.setOptions({
    lang: {
        drillUpText: '< Back'
    }
});

$.ajax({
    url: '/data/profile/match_history/' + Uid,
    dataType: 'json',
    success: function (json) {
        match_history = json["match_history"];

        active_classification = "PvP";
        active_game_type = "Total";

        populate_overview_totals();

        populate_filter_dropdowns();
        build_classification_list();
        build_game_type_list();
        populate_gamemode_overview();

        populate_match_history_table();

        $('#gamemode_overview_card').removeClass('d-none');
        $('#match_history_overview_card').removeClass('d-none');
    }
});

$.ajax({
url: '/data/profile/overview_drilldowns/' + Uid,
dataType: 'json',
success: function (json) {
    build_drilldown('gamemode_overview', 'Game Modes', json["gamemode_preference"]);
    build_drilldown('weapons_overview', 'Weapons', json["weapon_preference"]);
    build_drilldown('movement_overview', 'Movement', json["movement_preference"]);
}
});

$('#classification_list').on('click', 'a', function (e) {
    e.preventDefault();
    $(this).tab('show');
    active_classification = $(this).text();
    active_game_type = "Total";
    build_game_type_list();
    populate_gamemode_overview();
});

$('#game_type_list').on('click', 'a', function (e) {
    e.preventDefault();
    $(this).tab('show');
    active_game_type = $(this).text();
    populate_gamemode_overview();
});

$('#reset_filters').click(function (e) {
    $("div[id*=_selection_menu] a").each(function (index, element) {
        if ($(this).hasClass('active'))
            $(this).removeClass('active');
    });

    populate_gamemode_overview();
});

$('.dropdown-menu').click(function (e) {
    e.stopPropagation();
});

$(".dropdown-menu").on('click', 'a.dropdown-item', function (e) {
    e.stopPropagation();
    e.stopImmediatePropagation();
    $(this).toggleClass('active');
    clearTimeout(filter_delay);

    filter_delay = setTimeout(function () {
        populate_gamemode_overview();
    }, 350);
});

function populate_overview_totals() {

    let overview_data = new Stats();

    for (var i = 0; i < match_history.length; i++) 
        overview_data.add_game(match_history[i]);

    $('#total_games_recorded').text(overview_data.games);
    $('#total_time_recorded').text(overview_data.time_spent);
    $('#total_win_rate').text(overview_data.win_rate);
    $('#total_kag').text(overview_data.kda);
    $('#total_mvp_rate').text(overview_data.mvp_rate);

    $('#summary_row_1').removeClass('d-none');
    $('#summary_row_2').removeClass('d-none');

    if ($('#known_as').innerHTML != "") {
        $('#known_as').removeClass('d-none');
    }
}

function populate_match_history_table() {
    for (var i = 0; i < match_history.length; i++) {
        var start = getAdjustedTimestamp(match_history[i]["match_start"]);
        var row = $("<tr>");
        var cols = "";

        cols += '<td>' + match_history[i]["match_type"] + '</td>';
        cols += '<td><a href="/match/' + match_history[i]["match_id"] + '">' + start + '</a></td>';
        cols += '<td>' + match_history[i]["map"] + '</td>';
        cols += '<td>' + match_history[i]["power_score"] + '</td>';
        cols += '<td>' + match_history[i]["score"] + '</td>';
        cols += '<td>' + match_history[i]["kills"] + '</td>';
        cols += '<td>' + match_history[i]["assists"] + '</td>';
        cols += '<td>' + (match_history[i]["damage"]).toFixed(0) + '</td>';
        cols += '<td>' + (match_history[i]["damage_rec"]).toFixed(0) + '</td>';
        cols += '<td>' + match_history[i]["result"] + '</td>';

        cols += '<td></td>';
        row.append(cols);
        $('#match_history_body').append(row);
    }

    var domOption =
        "<'row m-1'<'d-inline-flex justify-content-start'p><'d-inline-flex ml-auto text-secondary'l>>" +
        "<tr>" +
        "<'row m-1'<'d-inline-flex justify-content-start'p><'d-none d-sm-inline-flex ml-auto text-secondary'i>>";

    var table = $('#match_history_table').DataTable({
        order: [[1, 'desc']],
        lengthMenu: [[10, 20, 50, -1], [10, 20, 50, "All"]],
        pagingType: "simple_numbers",
        dom: domOption,
        paging: true,
        searching: true,
        search: {
            smart: false,
            regex: false
        },
    });
}

function populate_filter_dropdowns() {
    var cabins = [];
    var hardware = [];
    var movement = [];
    var weapons = [];

    match_history.forEach(build => {
        build["parts"].split(',').forEach(part_string => {
            var parts = part_string.split(':');

            if (parts[0] === 'Cabins' && !cabins.includes(parts[1]))
                cabins.push(parts[1]);

            if (parts[0] === 'Hardware' && !hardware.includes(parts[1]))
                hardware.push(parts[1]);

            if (parts[0] === 'Movement' && !movement.includes(parts[1]))
                movement.push(parts[1]);

            if (parts[0] === 'Weapons' && !weapons.includes(parts[1]))
                weapons.push(parts[1]);
        });
    });

    cabins.forEach(x => {
        $('#cabin_part_selection_menu').append('<a class="dropdown-item" data-keyname="' + x + '">' + x + '</a>');
    });

    hardware.forEach(x => {
        $('#weapon_part_selection_menu').append('<a class="dropdown-item" data-keyname="' + x + '">' + x + '</a>');
    });

    movement.forEach(x => {
        $('#movement_part_selection_menu').append('<a class="dropdown-item" data-keyname="' + x + '">' + x + '</a>');
    });

    weapons.forEach(x => {
        $('#weapon_part_selection_menu').append('<a class="dropdown-item" data-keyname="' + x + '">' + x + '</a>');
    });
}

function populate_gamemode_overview() {
    let gamemode_data = new Stats();
    var min_date = new Date(8640000000000000);
    var parts = [];
    
    damage_list = [];
    dmg_rec_list = [];
    kd_list = [];
    score_list = [];

    $("div[id*=part_selection_menu] a").each(function (index, element) {
        if ($(this).hasClass('active'))
            parts.push($(this).attr("data-keyname"));
    });

    for (var i = 0; i < match_history.length; i++) {

        if (active_classification != 'Total' && active_classification != match_history[i]["match_classification"])
            continue;

        if (active_game_type != 'Total' && active_game_type != match_history[i]["match_type"])
            continue;

        if (parts && parts.length > 0 && match_history[i]["parts"] != null) {
            let found_part = false;
            parts.forEach(x => {
                if (match_history[i]["parts"].includes(x)) {
                    found_part = true;
                }
            });
            if (!found_part)
                continue;
        }

        gamemode_data.add_game(match_history[i]);

        damage_list.push(match_history[i]["damage"]);
        dmg_rec_list.push(match_history[i]["damage_rec"]);
        kd_list.push(match_history[i]["kills"] + match_history[i]["assists"]);
        score_list.push(match_history[i]["score"]);
    }

    damage_list = damage_list.sort(function (a, b) { return a - b });
    dmg_rec_list = dmg_rec_list.sort(function (a, b) { return a - b });
    kd_list = kd_list.sort(function (a, b) { return a - b });
    score_list = score_list.sort(function (a, b) { return a - b });

    $('#games_recorded').text(gamemode_data.games);
    $('#win_rate').text(gamemode_data.win_rate);
    $('#kills').text(gamemode_data.kills);
    $('#assists').text(gamemode_data.assists);
    $('#ka_g').text(gamemode_data.kda);
    $('#medals').text(gamemode_data.medal);
    $('#mvp').text(gamemode_data.mvp_rate);

    $('#avg_kills').text(gamemode_data.avg_kills);
    $('#avg_assists').text(gamemode_data.avg_assists);
    $('#avg_dmg').text(gamemode_data.avg_damage);
    $('#avg_dmg_rec').text(gamemode_data.avg_damage_rec);
    $('#avg_score').text(gamemode_data.avg_score);

    build_boxplot('dmg_box_plot', ['Dmg', 'Dmg Rec'], [boxplot_distribution(damage_list), boxplot_distribution(dmg_rec_list)]);
    build_boxplot('kill_box_plot', ['KD'], [boxplot_distribution(kd_list)]);
    build_boxplot('score_box_plot', ['Score'], [boxplot_distribution(score_list)]);
}

function build_classification_list() {
    var li = document.createElement('li');
    var match_classification = [];

    document.getElementById("classification_list").innerHTML = "";
    
    for (var i = 0; i < match_history.length; i++) {
        if (!match_classification.includes(match_history[i]["match_classification"]))
            match_classification.push(match_history[i]["match_classification"]);
    }
    if (match_classification.length > 1) {
        li.classList.add('nav-item');
        li.innerHTML = '<a class="nav-link" id="total-tab" data-toggle="pill" href="#pills-total" role="tab" aria-controls="total" aria-selected="false">Total</a>';
        document.getElementById('classification_list').appendChild(li);
    }
    
    for (var i = 0; i < match_classification.length; i++) {
        li = document.createElement('li');
        li.classList.add('nav-item');
        if (match_classification[i] == 'PvP') {
            li.innerHTML = '<a class="nav-link active" id="' + match_classification[i] + '-tab" data-toggle="pill" href="#pills-' + match_classification[i] + '" role="tab" aria-controls="pills-' + match_classification[i]+'" aria-selected="true">' + match_classification[i] + '</a>';
        }
        else {
            li.innerHTML = '<a class="nav-link" id="' + match_classification[i] + '-tab" data-toggle="pill" href="#pills-' + match_classification[i] + '" role="tab" aria-controls="pills-' + match_classification[i] +'" aria-selected="false">' + match_classification[i] + '</a>';
        } 

        document.getElementById('classification_list').appendChild(li);
    }
}

function build_game_type_list() {
    var li = document.createElement('li');
    var game_types = [];

    document.getElementById("game_type_list").innerHTML = "";
    
    for (var i = 0; i < match_history.length; i++) {
        if (!game_types.includes(match_history[i]["match_type"]) && match_history[i]["match_classification"] == active_classification)
            game_types.push(match_history[i]["match_type"]);
    }

    if (game_types.length > 1) {
        li.classList.add('nav-item');
        li.innerHTML = '<a class="nav-link active" id="total-tab" data-toggle="pill" href="#total" role="tab" aria-controls="total" aria-selected="true">Total</a>';
        document.getElementById('game_type_list').appendChild(li);
    }

    for (var i = 0; i < game_types.length; i++) {
        li = document.createElement('li');
        li.classList.add('nav-item');
        li.innerHTML = '<a class="nav-link" id="' + game_types[i] + '-tab" data-toggle="tab" href="#' + game_types[i] + '" role="tab" aria-controls="' + game_types[i] + '" aria-selected="false">' + game_types[i] + '</a>';

        document.getElementById('game_type_list').appendChild(li);
    }
}

function build_drilldown(id, title, drilldown_data) {

    var series_data = populate_series_data(drilldown_data);
    var drilldown_series_data = populate_drilldown_data(drilldown_data);
    var favorite = series_data.reduce((a, b) => a.y > b.y ? a : b);

    var chart = Highcharts.chart(id, {
        chart: {
            type: 'pie'
        },
        title: {
            text: title
        },
        credits: {
            enabled: false
        },
        subtitle: {
            text: 'Preference ' + favorite.name + ' (' + Math.round(favorite.y, 2) + '%)'
        },
        accessibility: {
            announceNewData: {
                enabled: true
            },
            point: {
                valueSuffix: '%'
            }
        },
        plotOptions: {
            pie: {
                borderColor: null,
                dataLabels: {
                    enabled: true,
                    distance: -50,
                    style: {
                        fontWeight: 'bold',
                        color: 'white'
                    },
                    formatter: function () {

                        if (this.point.isNull)
                            return void 0;

                        if (this.point.y < 10)
                            return void 0;

                        return this.point.name;
                    }
                }
            }
        },
        tooltip: {
            headerFormat: '<span style="font-size:11px">{point.name}</span><br>',
            pointFormat: '<span style="color:{point.color}">{point.name}</span>: <b>{point.y:.1f}%</b> of total<br/>'
        },
        series: [
            {
                name: title,
                colorByPoint: true,
                data: series_data
            }
        ],
        drilldown: {
            drillUpButton: {
                theme: {
                    fill: 'transparent',
                    'stroke-width': 0,
                    r: 2
                }
            },
            series: drilldown_series_data
        }
    });

    return chart;
}

function build_boxplot(id, categories, boxplot_data) {
    var height = (categories.length * 15) + '%';
    var width = $("#averages_card").width();

    Highcharts.chart(id, {
        chart: {
            type: 'boxplot',
            inverted: true,
            height: height,
            width: width
        },
        title: {
            text: null
        },
        credits: {
            enabled: false
        },
        legend: {
            enabled: false
        },
        xAxis: {
            categories: categories
        },
        yAxis: {
            title: {
                text: null
            }
        },
        plotOptions: {
            boxplot: {
                //fillColor: '#e2e5e8',
                lineWidth: 5,
                medianWidth: 5,
                stemWidth: 5,
                whiskerLength: '80%',
                whiskerWidth: 5
            }
        },
        series: [{
            name: null,
            pointPadding: 0,
            groupPadding: 0.2,
            borderWidth: 0,
            data: boxplot_data,
            tooltip: {
                headerFormat: '<em>{point.key}</em><br/>'
            }
        }]
    });
}

$('#averages_card').on('shown.bs.collapse', function () {
    build_boxplot('dmg_box_plot', ['Dmg', 'Dmg Rec'], [boxplot_distribution(damage_list), boxplot_distribution(dmg_rec_list)]);
    build_boxplot('kill_box_plot', ['KD'], [boxplot_distribution(kd_list)]);
    build_boxplot('score_box_plot', ['Score'], [boxplot_distribution(score_list)]);
});

function populate_series_data(drilldown_data) {
    var data = [];
    var total = 0;

    for (let i = 0; i < drilldown_data.length; i++) {
        total += drilldown_data[i]["count"];
    }

    for (let i = 0; i < drilldown_data.length; i++) {
        if (data.filter(function (e) { return e.name === drilldown_data[i]["type"]; }).length > 0) {
            data[data.findIndex(x => x.name === drilldown_data[i]["type"])].y += (drilldown_data[i]["count"] / total) * 100;
        }
        else {
            data.push({
                name: drilldown_data[i]["type"],
                y: (drilldown_data[i]["count"] / total) * 100,
                drilldown: drilldown_data[i]["type"]
            });
        }
    }
    return data;
}

function populate_drilldown_data(drilldown_data) {
    var data = [];
    var total = 0;

    for (let i = 0; i < drilldown_data.length; i++) {
        total += drilldown_data[i]["count"];
    }

    for (let i = 0; i < drilldown_data.length; i++) {
        var found_series = false;

        for (let j = 0; j < data.length; j++) {
            if (data[j].name == drilldown_data[i]["type"]) {
                data[j].data.push(
                    [drilldown_data[i]["name"], (drilldown_data[i]["count"] / total) * 100]
                );
                found_series = true;
                break;
            }
        }

        if (!found_series) {
            data.push({
                'name': drilldown_data[i]["type"],
                'id': drilldown_data[i]["type"],
                'data': [[drilldown_data[i]["name"], (drilldown_data[i]["count"] / total) * 100]]
            });
        }
    }

    return data;
}

function boxplot_distribution(data) {
    var distribution = [];
    var inter_quartile_range = quartile(data, 0.75) - quartile(data, 0.25);
    var minimum = quartile(data, 0.25) - 1.5 * inter_quartile_range;
    var maximum = quartile(data, 0.75) + 1.5 * inter_quartile_range;

    if (minimum < 0)
        minimum = 0;

    if (maximum > data[data.length - 1])
        maximum = data[data.length - 1];

    distribution.push(minimum);
    distribution.push(quartile(data, 0.25));
    distribution.push(quartile(data, 0.50));
    distribution.push(quartile(data, 0.75));
    distribution.push(maximum);

    return distribution;
}

function quartile(data, q) {
    var pos = ((data.length) - 1) * q;
    var base = Math.floor(pos);
    var rest = pos - base;
    if ((data[base + 1] !== undefined)) {
        return data[base] + rest * (data[base + 1] - data[base]);
    } else {
        return data[base];
    }
}