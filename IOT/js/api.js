;(function (window) {

	var api = window.API || {};
	// const REPO_URL = 'https://repository.dcrgraphs.net/api/';
	const REPO_URL = '/api/ProcessEngine/';

    api.initializeGraph = function (id, data) {
        return $.ajax({
            type: "POST",
            url: REPO_URL + 'Initialize/'+parseInt(id) ,
        });
    }
	
	api.getEvents = function (graphId, simId) {
		return $.ajax({
					type: "GET",
					contentType: "application/json; charset=utf-8",
		            url: REPO_URL + 'getevents/'+parseInt(graphId)+'/'+parseInt(simId),
					dataType: "json"
		        });
	}
	
	api.executeEvent = function (graphId, simId, event) {
		return $.ajax({
					type: "POST",
					contentType: "application/json; charset=utf-8",
		            url: REPO_URL + 'executeEvent/'+parseInt(graphId)+'/'+parseInt(simId),
					dataType: "json",
					data: JSON.stringify(event)
		        });
	}
	api.advanceTime = function (graphId, simId, time) {
		return $.ajax({
					type: "POST",
					contentType: "application/json; charset=utf-8",
		            url: REPO_URL + 'advancetime/'+parseInt(graphId)+'/'+parseInt(simId),
					dataType: "json",
					// data: JSON.stringify(eventData)
		        });
	}

	window.API = api;
}(window))