
//synergyWrapper

//const { each } = require("jquery");

// VARS
var synergyData = {
    data: {},
    loaded: false
};

var synergies = {
    tree: {
        topToBottom: [],
        visible: []
    },
};

// INIT
$(document).ready(function () {

});

function onSynergyDataLoaded() {
/*
    synergyList = synergyData.data.synergies;
    itemList = synergyData.data.synergyitems;
    uniqueId = 0;
    synergyList.forEach(function (s, i) {
        mapSynergyItems(null, s, 0, uniqueId);
        uniqueId++;
        itemList.forEach(function (e, i) {
            if (e.synergyType == s.synergyType) {
                mapSynergyItems(s, e, 1, uniqueId);
                uniqueId++;
            }
        });
    });
    drawSynergy();
*/

    var wrapper = $('#synergyWrapper').append('<div>');
    //wrapper.children().remove();
    var treeWrapper = $('<div class="col-4">' + 'TEST' + '</div>').appendTo(wrapper);
}

function mapSynergyItems(rootDisplayItem, item, currentDepth, uniqueId) {
    var displayItem = {
        uniqueId: uniqueId,
        synergyType: item.synergyType,
        itemNumber: item.itemNumber,
        name: item.itemNumber,
        factionId: 1,
        show: true,
        expanded: true,
        depth: currentDepth,
        hasIngredients: false,
        rootDisplayItem: rootDisplayItem,
    }

    if (displayItem.depth == 0) {
        displayItem.name = item.synergyType;
        displayItem.hasIngredients = true;
        expanded = true;
    }
    synergies.tree.topToBottom.push(displayItem);
}

function drawSynergy() {
    var wrapper = $('#synergyWrapper').append('<div>');
    wrapper.children().remove();
    var treeWrapper = $('<div class="col-4"></div>').appendTo(wrapper);
    drawSynergyTreeHeader(treeWrapper);
    synergies.tree.topToBottom.forEach(function (e, i) {
        if (e.show) {
            drawSynergyTreeEntry(e, treeWrapper);
            synergies.tree.visible.push(e);
        }
    });
    bindEvents();
    $('[data-toggle="tooltip"]').tooltip();
}

function drawSynergyTreeHeader(wrapper) {
    var html = '<div class="d-flex flex-row justify-content-between my-1 mx-1">' +
        '<div class="d-flex flex-row justify-content-between w-50">' +
        '<div class="font-weight-bold">' +
        '' +
        '</div>' +
        '</div>'
    $(wrapper).append(html);
}

function drawSynergyTreeEntry(displayItem, wrapper) {
    var depthSpacer = '';
    var expandButton = '<button class="btn btn-sm btn-outline-secondary recipe-expand-btn text-monospace ' + (displayItem.hasIngredients ? '' : 'invisible') + '" data-uniqueid="' + displayItem.uniqueId + '">' + (displayItem.expanded ? '-' : '+') + '</button>';

    var html = '<div class="d-flex flex-row justify-content-between my-1 mx-1"">' +

        '<div class="d-flex flex-row w-50">' +
        depthSpacer +
        (displayItem.rootDisplayItem !== null ? expandButton : '') +
        '<a href="/item/' + displayItem.itemNumber + '">' +
        '<div class="d-flex flex-row">' +
        '<img class="ml-1 item-image-med" src="' +
        '/img/items/' + displayItem.itemNumber + '.png' +
        '"/ >' +
        '<div class="ml-1">' +
        displayItem.name +
        '</div>' +
        (displayItem.factionId && displayItem.factionId > 0 && displayItem.hasIngredients ? '<div class="ml-1">' + '<img class="faction-icon" width="32" height="32" src="/img/faction-icons/' + displayItem.factionId + '.png" data-toggle="tooltip" data-placement="bottom" title="' + displayItem.factionName + '">' + '</div>' : '') +
        '</div>' +
        '</a>';
    $(wrapper).append(html);
}

