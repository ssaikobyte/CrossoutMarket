var Uid = window.location.pathname.split("/").pop();
var overview_totals = null;
var match_history = [];
var build_list = [];
var active_classification = null;
var active_game_type = null;
var damage_list = [];
var dmg_rec_list = [];
var kd_list = [];
var score_list = [];

Highcharts.setOptions({
    lang: {
        drillUpText: '< Back'
    }
});

$.ajax({
    url: '/data/profile/totals/' + Uid,
    dataType: 'json',
    success: function (json) {
        overview_totals = json;
        populate_overview_totals();
    }
});

$.ajax({
    url: '/data/profile/match_history/' + Uid,
    dataType: 'json',
    success: function (json) {
        match_history = json["match_history"];
        build_list = json["builds"];

        active_classification = "PvP";
        active_game_type = "Total";

        populate_part_dropdowns();
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

function populate_overview_totals() {
    $('#total_games_recorded').text(overview_totals["GamesRecorded"]);
    $('#total_time_recorded').text(overview_totals["TimePlayed"]);
    $('#total_win_rate').text(overview_totals["WinRate"]);
    $('#total_kag').text(overview_totals["KPB"]);
    $('#total_mvp_rate').text(overview_totals["MVPRate"]);
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
}

function populate_part_dropdowns() {

    var cabins = [];
    var hardware = [];
    var movement = [];
    var weapons = [];

    build_list.forEach(build => {
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
        $('#cabin_menu').append('<a class="dropdown-item" data-keyname="' + x + '">' + x + '</a>');
    });

    hardware.forEach(x => {
        $('#hardware_menu').append('<a class="dropdown-item" data-keyname="' + x + '">' + x + '</a>');
    });

    movement.forEach(x => {
        $('#movement_menu').append('<a class="dropdown-item" data-keyname="' + x + '">' + x + '</a>');
    });

    weapons.forEach(x => {
        $('#weapon_menu').append('<a class="dropdown-item" data-keyname="' + x + '">' + x + '</a>');
    });
}

function populate_gamemode_overview() {
    var games = 0;
    var rounds = 0;
    var wins = 0;
    var time_spent = 0;
    var medals = 0;
    var mvp = 0;
    var kills = 0;
    var assists = 0;
    var drone_kills = 0;
    var deaths = 0;
    var damage = 0;
    var damage_rec = 0;
    var score = 0;

    damage_list = [];
    dmg_rec_list = [];
    kd_list = [];
    score_list = [];

    for (var i = 0; i < match_history.length; i++) {

        if (active_classification != 'Total' && active_classification != match_history[i]["match_classification"])
            continue;

        if (active_game_type != 'Total' && active_game_type != match_history[i]["match_type"])
            continue;

        games += 1;
        rounds += match_history[i]["rounds"];

        if (match_history[i]["result"] === "Win")
            wins += 1;

        time_spent += match_history[i]["time_spent"];

        if (match_history[i]["medal_list"] != null) {
            match_history[i]["medal_list"].split(',').forEach(x => {
                var medal = x.split(':');
                medals += parseInt(medal[1]);

                if (medal[0] == 'PvpMvpWin')
                    mvp += parseInt(medal[1]); 
            });
        }

        kills += match_history[i]["kills"];
        assists += match_history[i]["assists"];
        drone_kills += match_history[i]["drone_kills"];
        deaths += match_history[i]["deaths"];
        damage += match_history[i]["damage"];
        damage_rec += match_history[i]["damage_rec"];
        score += match_history[i]["score"];

        damage_list.push(match_history[i]["damage"]);
        dmg_rec_list.push(match_history[i]["damage_rec"]);
        kd_list.push(match_history[i]["kills"] + match_history[i]["assists"]);
        score_list.push(match_history[i]["score"]);
    }

    damage_list = damage_list.sort(function (a, b) { return a - b });
    dmg_rec_list = dmg_rec_list.sort(function (a, b) { return a - b });
    kd_list = kd_list.sort(function (a, b) { return a - b });
    score_list = score_list.sort(function (a, b) { return a - b });

    $('#games_recorded').text(games);
    $('#win_rate').text(((wins / games) * 100).toFixed(1) + '%');
    $('#kills').text(kills);
    $('#assists').text(assists);
    $('#ka_g').text(((kills + assists) / rounds).toFixed(2));
    $('#medals').text(medals);
    $('#mvp').text(((mvp / rounds) * 100).toFixed(1) + '%');

    $('#avg_kills').text((kills / rounds).toFixed(2));
    $('#avg_assists').text((assists / rounds).toFixed(2));
    $('#avg_dmg').text((damage / rounds).toFixed(0));
    $('#avg_dmg_rec').text((damage_rec / rounds).toFixed(0));
    $('#avg_score').text((score / rounds).toFixed(0));

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

//$("#expand_gamemode_visualization").on('click', function (e) {

//    var expanded = $('#averages_card').attr("aria-expanded");

//    console.log(expanded);

//    ///* OPTIONAL: some variables you can use in your chart options for custom sizing */
//    //var chartWidth = $("#kanwil-report-kepuasan").width();
//    //var chartHeight = $("#kanwil-report-kepuasan").height();

//    ///* draw the chart once the tab has been clicked and it is now visible */
//    //$('#kanwil-report-kepuasan').highcharts({
//    //    /* your chart options go here */
//    //});

//    //e.stopPropagation();

//});

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