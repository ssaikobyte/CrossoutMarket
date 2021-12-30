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
        this.score_aggregate = new Aggregate();
        this.kills_aggregate = new Aggregate();
        this.assists_aggregate = new Aggregate();
        this.deaths_aggregate = new Aggregate();
        this.drone_kills_aggregate = new Aggregate();
        this.medals_aggregate = new Aggregate();
        this.damage_aggregate = new Aggregate();
        this.damage_rec_aggregate = new Aggregate();
        this.time_aggregate = new Aggregate();
        this.gamemode = [];
        this.weapons = [];
        this.movement = [];
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

        this.score_aggregate.check_aggregates(game["match_id"], game["score"]);
        this.kills_aggregate.check_aggregates(game["match_id"], game["kills"]);
        this.assists_aggregate.check_aggregates(game["match_id"], game["assists"]);
        this.deaths_aggregate.check_aggregates(game["match_id"], game["deaths"]);
        this.drone_kills_aggregate.check_aggregates(game["match_id"], game["drone_kills"]);
        this.medals_aggregate.check_aggregates(game["match_id"], medal);
        this.damage_aggregate.check_aggregates(game["match_id"], game["damage"]);
        this.damage_rec_aggregate.check_aggregates(game["match_id"], game["damage_rec"]);
        this.time_aggregate.check_aggregates(game["match_id"], game["time_spent"]);
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

class Aggregate {
    constructor() {
        this.total = 0;
        this.count = 0;
        this.minimum = 999999;
        this.mimimum_count = 0;
        this.minimum_guid = 0;
        this.maximum = 0;
        this.maximum_count = 0;
        this.maximum_guid = 0;
    }

    check_aggregates(guid, value) {
        this.total += value;
        this.count += 1;

        if (value < this.minimum) {
            this.minimum = value;
            this.mimimum_count = 1;
            this.minimum_guid = guid;
        }
        else if (value === this.minimum) {
            this.mimimum_count += 1;
        }

        if (value > this.maximum) {
            this.maximum = value;
            this.maximum_count = 1;
            this.maximum_guid = guid;
        }
        else if (value === this.maximum) {
            this.maximum_count += 1;
        }
    }

    get total_text() {
        return (this.total).toFixed(0).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","); 
    }

    get min_text() {
        let text = '<a href="/match/' + this.minimum_guid + '" class="text-dark link-secondary">' + this.minimum.toFixed(0).replace(/\B(?=(\d{3})+(?!\d))/g, ",") + '</a>';

        if (this.minimum_count > 1)
            text += "<div class=\"lead\" style=\"font - size: 0.4rem;\">x" + this.minimum_count + "</div>";

        return text;
    }

    get max_text() {
        let text = '<a href="/match/' + this.maximum_guid + '" class="text-dark link-secondary">' + this.maximum.toFixed(0).replace(/\B(?=(\d{3})+(?!\d))/g, ",") + '</a>';

        if (this.minimum_count > 1)
            text += "<div class=\"lead\" style=\"font - size: 0.4rem;\">x" + this.maximum_count + "</div>";

        return text;
    }

    get avg_text() {
        let avg = this.total / this.count;

        if (avg > 10) {
            return (avg).toFixed(0).replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        }

        return (avg).toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    }
}

class FilterItem {
    constructor(name, count) {
        this.name = name;
        this.count = count;
        this.selected = false;
    }

    increment() {
        this.count += 1;
    }
}

class StatFilter {
    constructor() {
        this.categories = [];
        this.match_types = [];
        this.start_time = null;
        this.end_time = null;
        this.region = [];
        this.hosts = [];
        this.client_version = [];
        this.maps = [];
        this.group_size = [];
        this.group = [];
        this.power_score = [];
        this.hardware = [];
        this.cabins = [];
        this.movement = [];
        this.weapons = [];
        this.decor = [];
        this.structure = [];
        this.game_result = [];
        this.survived = [];
    }

    add_or_increment(category, name, count = 1) {
        let found = category.find((o, i) => {
            if (o.name === name) {
                category[i].increment();
                return true;
            }
        });

        if (!found)
            category.push(new FilterItem(name, count));
    }

    reset() {
        this.categories.forEach(x => { x.count = 0 });
        this.match_types.forEach(x => { x.count = 0 });
        this.region.forEach(x => { x.count = 0 });
        this.hosts.forEach(x => { x.count = 0 });
        this.client_version.forEach(x => { x.count = 0 });
        this.maps.forEach(x => { x.count = 0 });
        this.group_size.forEach(x => { x.count = 0 });
        this.group.forEach(x => { x.count = 0 });
        this.power_score.forEach(x => { x.count = 0 });
        this.hardware.forEach(x => { x.count = 0 });
        this.cabins.forEach(x => { x.count = 0 });
        this.movement.forEach(x => { x.count = 0 });
        this.weapons.forEach(x => { x.count = 0 });
        this.decor.forEach(x => { x.count = 0 });
        this.structure.forEach(x => { x.count = 0 });
        this.game_result.forEach(x => { x.count = 0 });
        this.survived.forEach(x => { x.count = 0 });
    }

    reset_selected() {
        this.start_time = null;
        this.end_time = null;
        this.categories.forEach(x => { x.selected = false });
        this.match_types.forEach(x => { x.selected = false });
        this.region.forEach(x => { x.selected = false });
        this.hosts.forEach(x => { x.selected = false });
        this.client_version.forEach(x => { x.selected = false });
        this.maps.forEach(x => { x.selected = false });
        this.group_size.forEach(x => { x.selected = false });
        this.group.forEach(x => { x.selected = false });
        this.power_score.forEach(x => { x.selected = false });
        this.hardware.forEach(x => { x.selected = false });
        this.cabins.forEach(x => { x.selected = false });
        this.movement.forEach(x => { x.selected = false });
        this.weapons.forEach(x => { x.selected = false });
        this.decor.forEach(x => { x.selected = false });
        this.structure.forEach(x => { x.selected = false });
        this.game_result.forEach(x => { x.selected = false });
        this.survived.forEach(x => { x.selected = false });
    }

    populate_static() {
        this.add_or_increment(this.power_score, "0-2499", 0);
        this.add_or_increment(this.power_score, "2500-3499", 0);
        this.add_or_increment(this.power_score, "3500-4499", 0);
        this.add_or_increment(this.power_score, "4500-5499", 0);
        this.add_or_increment(this.power_score, "5500-6499", 0);
        this.add_or_increment(this.power_score, "6500-7499", 0);
        this.add_or_increment(this.power_score, "7500-8499", 0);
        this.add_or_increment(this.power_score, "8500-9499", 0);
        this.add_or_increment(this.power_score, "9500-12999", 0);
        this.add_or_increment(this.power_score, "13000+", 0);
        this.add_or_increment(this.group_size, "Solo", 0);
        this.add_or_increment(this.group_size, "Any Group Size", 0);
        this.add_or_increment(this.group_size, "2 Man", 0);
        this.add_or_increment(this.group_size, "3 Man", 0);
        this.add_or_increment(this.group_size, "4 Man", 0);
        this.add_or_increment(this.survived, "Survived", 0);
        this.add_or_increment(this.survived, "Died", 0);
        this.add_or_increment(this.survived, "Unscathed", 0);
    }

    populate_filters(history) {
        this.populate_static();
        this.reset();
        history.forEach(match => {
            this.add_or_increment(this.categories, match["match_classification"]);
            this.add_or_increment(this.match_types, match["match_type"]);
            this.add_or_increment(this.maps, match["map"]);
            this.add_or_increment(this.hosts, match["host_name"]);
            this.add_or_increment(this.client_version, match["client_version"]);
            this.add_or_increment(this.game_result, match["result"]);
            this.add_or_increment(this.power_score, this.find_ps_range(match["power_score"]));
            this.add_or_increment(this.region, this.find_host_region(match["host_name"]));
            this.add_or_increment(this.survived, this.find_survived(match["deaths"], match["damage_rec"]));

            match["parts"].split(',').forEach(part_string => {
                var parts = part_string.split(':');

                if (parts[0] === 'Cabins') {
                    this.add_or_increment(this.cabins, parts[1]);
                }
                else
                if (parts[0] === 'Hardware') {
                    this.add_or_increment(this.hardware, parts[1]);
                }
                else
                if (parts[0] === 'Movement') {
                    this.add_or_increment(this.movement, parts[1]);
                }
                else
                if (parts[0] === 'Weapons') {
                    this.add_or_increment(this.weapons, parts[1]);
                }
                else
                if (parts[0] === 'Decor' || parts[0] === 'Customization') {
                    this.add_or_increment(this.decor, parts[1]);
                }
                else
                if (parts[0] === 'Structure' || parts[0] === 'Frames') {
                    this.add_or_increment(this.structure, parts[1]);
                }
            });
        });
    }

    set_dates(start_date, end_date) {
        this.start_time = start_date;
        this.end_time = end_date;
    }
    
    build_dropdowns() {
        this.build_dropdown_list('#game_category_selection_menu', this.categories);
        this.build_dropdown_list('#game_type_selection_menu', this.match_types);
        this.build_dropdown_list('#region_selection_menu', this.region);
        this.build_dropdown_list('#host_selection_menu', this.hosts);
        this.build_dropdown_list('#version_selection_menu', this.client_version);
        this.build_dropdown_list('#map_selection_menu', this.maps);
        this.build_dropdown_list('#power_score_selection_menu', this.power_score);
        this.build_dropdown_list('#group_selection_menu', this.group_size);
        this.build_dropdown_list('#cabin_part_selection_menu', this.cabins);
        this.build_dropdown_list('#hardware_part_selection_menu', this.hardware);
        this.build_dropdown_list('#movement_part_selection_menu', this.movement);
        this.build_dropdown_list('#weapon_part_selection_menu', this.weapons);
        this.build_dropdown_list('#decor_selection_menu', this.decor);
        this.build_dropdown_list('#structure_selection_menu', this.structure);
        this.build_dropdown_list('#game_result_selection_menu', this.game_result);
        this.build_dropdown_list('#survived_selection_menu', this.survived);
    }

    select_filter_item(parent, item, selected) {
        if (parent === 'game_category_selection_menu') {
            this.categories.find(x => x.name === item).selected = selected;
        }
        else if (parent === 'game_type_selection_menu') {
            this.match_types.find(x => x.name === item).selected = selected;
        }
        else if (parent === 'region_selection_menu') {
            this.region.find(x => x.name === item).selected = selected;
        }
        else if (parent === 'host_selection_menu') {
            this.hosts.find(x => x.name === item).selected = selected;
        }
        else if (parent === 'version_selection_menu') {
            this.client_version.find(x => x.name === item).selected = selected;
        }
        else if (parent === 'map_selection_menu') {
            this.maps.find(x => x.name === item).selected = selected;
        }
        else if (parent === 'group_selection_menu') {
            this.group_size.find(x => x.name === item).selected = selected;
        }
        else if (parent === 'power_score_selection_menu') {
            this.power_score.find(x => x.name === item).selected = selected;
        }
        else if (parent === 'cabin_part_selection_menu') {
            this.cabins.find(x => x.name === item).selected = selected;
        }
        else if (parent === 'hardware_part_selection_menu') {
            this.hardware.find(x => x.name === item).selected = selected;
        }
        else if (parent === 'movement_part_selection_menu') {
            this.movement.find(x => x.name === item).selected = selected;
        }
        else if (parent === 'weapon_part_selection_menu') {
            this.weapons.find(x => x.name === item).selected = selected;
        }
        else if (parent === 'decor_selection_menu') {
            this.decor.find(x => x.name === item).selected = selected;
        }
        else if (parent === 'structure_selection_menu') {
            this.structure.find(x => x.name === item).selected = selected;
        }
        else if (parent === 'game_result_selection_menu') {
            this.game_result.find(x => x.name === item).selected = selected;
        }
        else if (parent === 'survived_selection_menu') {
            this.survived.find(x => x.name === item).selected = selected;
        }
    }

    build_dropdown_list(element, list) {
        list.forEach(x => {
            $(element).append('<a class="dropdown-item" data-keyname="' + x.name + '" title= "' + x.count + ' Results">' + x.name + '</a>');
        });
    }

    build_title() {
        let title = "";

        if (this.categories.some(x => x.selected === true)) {
            if (this.categories.filter(x => x.selected === true).length > 1)
                title = title.concat('(', this.categories.filter(x => x.selected === true).map(x => x.name).join(", "), ') ');
            else 
                title = title.concat(this.categories.filter(x => x.selected === true).map(x => x.name).join(", "), ' ');

            if (this.match_types.some(x => x.selected === true))
                title = title.concat('Limited to ');
        }

        if (this.match_types.some(x => x.selected === true)) {
            if (this.match_types.filter(x => x.selected === true).length > 1)
                title = title.concat('(', this.match_types.filter(x => x.selected === true).map(x => x.name).join(", "), ') ');
            else 
                title = title.concat(this.match_types.filter(x => x.selected === true).map(x => x.name).join(", "), ' ');
        }

        if (this.start_time != null && this.end_time != null) {
            title = title.concat('From ', this.start_time.format('YYYY-MM-DD'), ' To ', this.end_time.format('YYYY-MM-DD'), ' ');
        }

        if (this.region.some(x => x.selected === true)) {
            if (this.region.filter(x => x.selected === true).length > 1)
                title = title.concat('In (', this.region.filter(x => x.selected === true).map(x => x.name).join(", "), ') ');
            else 
                title = title.concat('In ', this.region.filter(x => x.selected === true).map(x => x.name).join(", "), ' ');
        }

        if (this.hosts.some(x => x.selected === true)) {
            if (this.hosts.filter(x => x.selected === true).length > 1)
                title = title.concat('On Servers (', this.hosts.filter(x => x.selected === true).map(x => x.name).join(", "), ') ');
            else 
                title = title.concat('On Server ', this.hosts.filter(x => x.selected === true).map(x => x.name).join(", "), ' ');
        }

        if (this.client_version.some(x => x.selected === true)) {
            if (this.client_version.filter(x => x.selected === true).length > 1)
                title = title.concat('Using Client Versions (', this.client_version.filter(x => x.selected === true).map(x => x.name).join(", "), ') ');
            else 
                title = title.concat('Using Client Version ', this.client_version.filter(x => x.selected === true).map(x => x.name).join(", "), ' ');
        }

        if (this.maps.some(x => x.selected === true)) {
            if (this.maps.filter(x => x.selected === true).length > 1)
                title = title.concat('On Maps (', this.maps.filter(x => x.selected === true).map(x => x.name).join(", "), ') ');
            else
                title = title.concat('On Map ', this.maps.filter(x => x.selected === true).map(x => x.name).join(", "), ' ');
        }

        if (this.power_score.some(x => x.selected === true)) {
            if (this.power_score.filter(x => x.selected === true).length > 1)
                title = title.concat('Within Power Score Ranges (', this.power_score.filter(x => x.selected === true).map(x => x.name).join(", "), ') ');
            else
                title = title.concat('Within Power Score Range ', this.power_score.filter(x => x.selected === true).map(x => x.name).join(", "), ' ');
        }

        if (this.cabins.some(x => x.selected === true) ||
            this.weapons.some(x => x.selected === true) ||
            this.movement.some(x => x.selected === true) ||
            this.hardware.some(x => x.selected === true) ||
            this.decor.some(x => x.selected === true) ||
            this.structure.some(x => x.selected === true)) {

            if (this.cabins.concat(this.weapons).concat(this.movement).concat(this.hardware).concat(this.decor).concat(this.structure).filter(x => x.selected === true).length > 1)
                title = title.concat('Using Builds Equipped With (', this.cabins.concat(this.weapons).concat(this.movement).concat(this.hardware).concat(this.decor).concat(this.structure).filter(x => x.selected === true).map(x => x.name).join(", "), ') ');
            else
                title = title.concat('Using ', this.cabins.concat(this.weapons).concat(this.movement).concat(this.hardware).concat(this.decor).concat(this.structure).filter(x => x.selected === true).map(x => x.name).join(", "), ' ');
        }

        if (this.game_result.some(x => x.selected === true)) {
            if (this.game_result.filter(x => x.selected === true).length > 1)
                title = title.concat('Resulting in a (', this.game_result.filter(x => x.selected === true).map(x => x.name).join(", "), ') ');
            else
                title = title.concat('Resulting in a ', this.game_result.filter(x => x.selected === true).map(x => x.name).join(", "), ' ');
        }

        if (this.survived.some(x => x.selected === true)) {
            if (this.survived.filter(x => x.selected === true).length > 1)
                title = title.concat('Where you (', this.survived.filter(x => x.selected === true).map(x => x.name).join(", "), ') ');
            else
                title = title.concat('Where you ', this.survived.filter(x => x.selected === true).map(x => x.name).join(", "), ' ');
        }

        if (title === "Matches ")
            title = "";

        return title;
    }

    valid_match(match) {
        let valid = true; 

        if (this.start_time !== null && this.end_time !== null) {
            if (Date.parse(match["match_start"]) < Date.parse(this.start_time.format('YYYY-MM-DD')) || Date.parse(match["match_start"]) > Date.parse(this.end_time.format('YYYY-MM-DD')))
                valid = false;
        }

        if (this.categories.some(x => x.selected === true)) {
            if (!this.categories.find(x => x.selected === true && x.name === match["match_classification"]))
                valid = false;
        }

        if (this.match_types.some(x => x.selected === true)) {
            if (!this.match_types.find(x => x.selected === true && x.name === match["match_type"]))
                valid = false;
        }

        if (this.region.some(x => x.selected === true)) {
            if (!this.region.find(x => x.selected === true && x.name === this.find_host_region(match["host_name"])))
                valid = false;
        }

        if (this.hosts.some(x => x.selected === true)) {
            if (!this.hosts.find(x => x.selected === true && x.name === match["host_name"]))
                valid = false;
        }

        if (this.client_version.some(x => x.selected === true)) {
            if (!this.client_version.find(x => x.selected === true && x.name === match["client_version"]))
                valid = false;
        }

        if (this.maps.some(x => x.selected === true)) {
            if (!this.maps.find(x => x.selected === true && x.name === match["map"]))
                valid = false;
        }

        if (this.power_score.some(x => x.selected === true)) {
            if (!this.power_score.find(x => x.selected === true && x.name === this.find_ps_range(match["power_score"])))
                valid = false;
        }

        if (this.cabins.some(x => x.selected === true) ||
            this.weapons.some(x => x.selected === true) ||
            this.movement.some(x => x.selected === true) ||
            this.hardware.some(x => x.selected === true) ||
            this.decor.some(x => x.selected === true) ||
            this.structure.some(x => x.selected === true)) {
            if (!this.cabins.concat(this.weapons).concat(this.movement).concat(this.hardware).concat(this.decor).concat(this.structure).find(x => x.selected === true && match["parts"].includes(x.name)))
                valid = false;
        }

        if (this.game_result.some(x => x.selected === true)) {
            if (!this.game_result.find(x => x.selected === true && x.name === match["result"]))
                valid = false;
        }

        if (this.survived.some(x => x.selected === true)) {
            if (!this.survived.find(x => x.selected === true && x.name === this.find_survived(match["deaths"], match["damage_rec"])))
                valid = false;
        }

        return valid;
    }

    find_ps_range(ps) {
        if (ps <= 2499) {
            return "0-2499";
        }
        else if (ps <= 3499) {
            return "2500-3499";
        }
        else if (ps <= 4499) {
            return "3500-4499";
        }
        else if (ps <= 5499) {
            return "4500-5499";
        }
        else if (ps <= 6499) {
            return "5500-6499";
        }
        else if (ps <= 7499) {
            return "6500-7499";
        }
        else if (ps <= 8499) {
            return "7500-8499";
        }
        else if (ps <= 9499) {
            return "8500-9499";
        }
        else if (ps <= 12999) {
            return "9500-12999";
        }
        else if (ps <= 22500) {
            return "13000+";
        }
        else if (ps > 22500) {
            return "Leviathian";
        }
        else {
            return "Unknown";
        }
    }

    find_host_region(host_name) {
        if (host_name.includes("-ru")) {
            return "Russia";
        }
        else if (host_name.includes("-nl")) {
            return "Europe";
        }
        else if (host_name.includes("-us")) {
            return "North America";
        }
        else if (host_name.includes("-jp")) {
            return "Asia";
        }
        else if (host_name.includes("-au")) {
            return "Australia";
        }
        else {
            return "Unknown";
        }
    }

    find_survived(deaths, damage_rec) {
        if (deaths === 0 && damage_rec < 1) {
            return "Unscathed";
        }
        else if (deaths === 0) {
            return "Survived";
        }
        else if (deaths > 0) {
            return "Died";
        }
        else {
            return "Uknown";
        }
    }
};

var Uid = window.location.pathname.split("/").pop();
let filter = new StatFilter();
let gamemode_data = new Stats();
let match_history = [];
let filter_delay;


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

        filter.populate_filters(match_history);
        filter.build_dropdowns();

        populate_profile();

        $('#gamemode_overview_card').removeClass('d-none');
        $('#match_history_overview_card').removeClass('d-none');
    }
});

$('#breakdown_list').on('click', 'a', function (e) {
    e.preventDefault();
    $(this).tab('show');
    console.log($(this).text());
    populate_aggregate_data();
});

$('#reset_filters').click(function (e) {
    $("div[id*=_selection_menu] a.active").removeClass('active');
    filter.reset_selected();
    filter.populate_filters(match_history);
    populate_profile();
});

$('.dropdown-menu').click(function (e) {
    e.stopPropagation();
});

$(".dropdown-menu").on('click', 'a.dropdown-item', function (e) {
    e.stopPropagation();
    e.stopImmediatePropagation();
    $(this).toggleClass('active');
    filter.select_filter_item($(this).parent().attr("id"), $(this).attr("data-keyname"), $(this).hasClass('active'));
    clearTimeout(filter_delay);

    filter_delay = setTimeout(function () {
        populate_profile();
    }, 350);
});

$(function () {
    $('input[name="daterange"]').daterangepicker({
        autoApply: true,
        drops: 'down',
        alwaysShowCalendars: true,
        startDate: moment().subtract(13, 'days'),
        endDate: moment(),
        opens: 'left',
        ranges: {
            'Today': [moment(), moment()],
            'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
            'Last 7 Days': [moment().subtract(6, 'days'), moment()],
            'Last 30 Days': [moment().subtract(29, 'days'), moment()],
            'This Month': [moment().startOf('month'), moment().endOf('month')],
            'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
        }
    }, function (start, end, label) {
        filter.set_dates(start, end);
        populate_profile();
    });
});

function populate_match_history_table() {
    var domOption =
        "<'row m-1'<'d-inline-flex justify-content-start'p><'d-inline-flex ml-auto text-secondary'l>>" +
        "<tr>" +
        "<'row m-1'<'d-inline-flex justify-content-start'p><'d-none d-sm-inline-flex ml-auto text-secondary'i>>";

    var table = $('#match_history_table').DataTable({
        order: [[1, 'desc']],
        destroy: true,
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

function append_match_to_history(match) {
    var start = getAdjustedTimestamp(match["match_start"]);
    var row = $("<tr>");
    var cols = "";

    cols += '<td>' + match["match_type"] + '</td>';
    cols += '<td><a href="/match/' + match["match_id"] + '">' + start + '</a></td>';
    cols += '<td>' + match["map"] + '</td>';
    cols += '<td>' + match["power_score"] + '</td>';
    cols += '<td>' + match["score"] + '</td>';
    cols += '<td>' + match["kills"] + '</td>';
    cols += '<td>' + match["assists"] + '</td>';
    cols += '<td>' + (match["damage"]).toFixed(0) + '</td>';
    cols += '<td>' + (match["damage_rec"]).toFixed(0) + '</td>';
    cols += '<td>' + match["result"] + '</td>';

    cols += '<td></td>';
    row.append(cols);
    $('#match_history_body').append(row);
}

function populate_profile() {
    let temp_history = [];

    gamemode_data = new Stats();
    $("#match_history_body tr").remove();
    $('#stat_title').text(filter.build_title());
    
    for (var i = 0; i < match_history.length; i++) {
        if (!filter.valid_match(match_history[i]))
            continue;

        gamemode_data.add_game(match_history[i]);
        append_match_to_history(match_history[i]);
        temp_history.push(match_history[i]);
    }

    filter.populate_filters(temp_history);

    $('#total_games_recorded').text(gamemode_data.games.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    $('#total_time_recorded').text(gamemode_data.time_spent);
    $('#total_win_rate').text(gamemode_data.win_rate);
    $('#total_kag').text(gamemode_data.kda);
    $('#total_mvp_rate').text(gamemode_data.mvp_rate);

    if ($('#known_as').innerHTML != "") 
        $('#known_as').removeClass('d-none');

    $('#summary_row_1').removeClass('d-none');
    $('#summary_row_2').removeClass('d-none');

    //build_drilldown('gamemode_overview', 'Game Modes', temp_history);
    //build_drilldown('weapons_overview', 'Weapons', temp_history);
    //build_drilldown('movement_overview', 'Movement', temp_history);

    populate_aggregate_data();
    populate_match_history_table();
}

function populate_aggregate_data() {
    let tab = $("ul#breakdown_list a.active").text();
    console.log(tab);
    if (tab === "Total") {
        document.getElementById("kills").innerHTML = gamemode_data.kills_aggregate.total_text;
        document.getElementById("assists").innerHTML = gamemode_data.assists_aggregate.total_text;
        document.getElementById("deaths").innerHTML = gamemode_data.deaths_aggregate.total_text;
        document.getElementById("damage").innerHTML = gamemode_data.damage_aggregate.total_text;
        document.getElementById("damage_recieved").innerHTML = gamemode_data.damage_rec_aggregate.total_text;
        document.getElementById("medals").innerHTML = gamemode_data.medals_aggregate.total_text;
        document.getElementById("score").innerHTML = gamemode_data.score_aggregate.total_text;
        document.getElementById("time").innerHTML = time_to_readable(gamemode_data.time_aggregate.total);
    }
    else if (tab === "Minimum") {
        document.getElementById("kills").innerHTML = gamemode_data.kills_aggregate.min_text;
        document.getElementById("assists").innerHTML = gamemode_data.assists_aggregate.min_text;
        document.getElementById("deaths").innerHTML = gamemode_data.deaths_aggregate.min_text;
        document.getElementById("damage").innerHTML = gamemode_data.damage_aggregate.min_text;
        document.getElementById("damage_recieved").innerHTML = gamemode_data.damage_rec_aggregate.min_text;
        document.getElementById("medals").innerHTML = gamemode_data.medals_aggregate.min_text;
        document.getElementById("score").innerHTML = gamemode_data.score_aggregate.min_text;
        document.getElementById("time").innerHTML = '<a href="/match/' + gamemode_data.time_aggregate.minimum_guid + '" class="text-dark link-secondary">' + time_to_readable(gamemode_data.time_aggregate.minimum) + '</a>';
    }
    else if (tab === "Maximum") {
        document.getElementById("kills").innerHTML = gamemode_data.kills_aggregate.max_text;
        document.getElementById("assists").innerHTML = gamemode_data.assists_aggregate.max_text;
        document.getElementById("deaths").innerHTML = gamemode_data.deaths_aggregate.max_text;
        document.getElementById("damage").innerHTML = gamemode_data.damage_aggregate.max_text;
        document.getElementById("damage_recieved").innerHTML = gamemode_data.damage_rec_aggregate.max_text;
        document.getElementById("medals").innerHTML = gamemode_data.medals_aggregate.max_text;
        document.getElementById("score").innerHTML = gamemode_data.score_aggregate.max_text;
        document.getElementById("time").innerHTML = '<a href="/match/' + gamemode_data.time_aggregate.maximum_guid + '" class="text-dark link-secondary">' + time_to_readable(gamemode_data.time_aggregate.maximum) + '</a>';
    }
    else if (tab === "Average") {
        document.getElementById("kills").innerHTML = gamemode_data.kills_aggregate.avg_text;
        document.getElementById("assists").innerHTML = gamemode_data.assists_aggregate.avg_text;
        document.getElementById("deaths").innerHTML = gamemode_data.deaths_aggregate.avg_text;
        document.getElementById("damage").innerHTML = gamemode_data.damage_aggregate.avg_text;
        document.getElementById("damage_recieved").innerHTML = gamemode_data.damage_rec_aggregate.avg_text;
        document.getElementById("medals").innerHTML = gamemode_data.medals_aggregate.avg_text;
        document.getElementById("score").innerHTML = gamemode_data.score_aggregate.avg_text;
        document.getElementById("time").innerHTML = time_to_readable((gamemode_data.time_aggregate.total /gamemode_data.time_aggregate.count).toFixed(0));
    }
}

function time_to_readable(time) {
    let total_seconds = time;
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