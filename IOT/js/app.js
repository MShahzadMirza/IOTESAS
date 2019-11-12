var App = (function(window, self) {

	self.version = '1.0.0',
    self.nextDeadline = null,
    self.currentTime = null,
    self.scheduler = null,
    self.simId = "87570",/**X-DCR-simulation-ID */
    self.graphId = null,
    self.callInProgress = false
    self.eventsToExecute = [
        {
            eventId: "Time",
            eventData: '<globalStore><variable id="Time" value="0.0:0:20" type="duration" isNull="false" /></globalStore>'
        },
        {
            eventId: "Before",
            eventData: '<globalStore><variable id="Before" value="0.0:00:10" type="duration" isNull="false" /></globalStore>'
        },
        {
            eventId: "After",
            eventData: '<globalStore><variable id="After" value="0.0:0:10" type="duration" isNull="false" /></globalStore>'
        }
    ];

	self.init = function (graphId) {
        console.log('Initiating App', self.eventsToExecute); 
        /**
         * 1. initialize graph, get its sim id, set is app sim id
         * 2. setup the basic fields with custom data
         * 3. setupScheduler, which will call getevents based on deadline
         * 4. execute Events
         */
        self.graphId = graphId
        //1. initialize graph, get simid /**X-DCR-simulation-ID */, set it in app
        API.initializeGraph(self.graphId).done(function(res){ 
            console.log(res)
            self.simId = parseInt(res);
            setupScheduler()
        }).fail(function (res) {
            console.error(res)
        })
        
        
    }

    //will setup the deadline and sim time in app
    function handleResponse(res) {
        let data = $.parseXML(res.replace("\"<", "<").replace(">\"", ">").replace(/\\/g, ''));
        let $data = $(data).find("events")
        self.nextDeadline = $data.attr("nextDeadline") || null;
        self.currentTime = $data.attr("currentTime") || null;
        $data.find("event").each(function (idx, evt) {
            if(evt.getAttribute("enabled") =="true"){
                self.eventsToExecute.push({
                    eventId: evt.getAttribute("id"),
                    eventData: null
                })
            };
        });
    }

    function executeEvent(event) { 
        console.log(event)
        API.executeEvent(self.graphId, self.simId, event).done(function(res){ 
            console.log(res, )
            //! setup Scehdualar again
            self.callInProgress = false;
            document.getElementById("bulb").className = event.eventId.toLowerCase()
            setupScheduler()
        }).fail(function (res) {
            console.error(res)
        })
        
    }

    function executionHandler() {
        /* 2. run basic setup and then run getevents to execute any pending event*/
        if(self.eventsToExecute.length>0){
            executeEvent(self.eventsToExecute.shift())
        }else{
            //TODO: 3. advance Time, to current system time
            console.log("execution Handler")
            //4. getEvents and handle and handleResponse(res)
            API.getEvents(self.graphId, self.simId).done(function(res){ 
                console.log(res)
                handleResponse(res)
                //5. execute any Pending Events
                //! setup Scehdualar again
                self.callInProgress = false;
                setupScheduler()
            }).fail(function (res) {
                console.error(res)
            })
        } 
    }

    //demo timer to display when the next call will be held
    function timer(localTime) {
        // Set the date we're counting down to
        var countDownDate = new Date(localTime).getTime();

        // Update the count down every 1 second
        var x = setInterval(function() {

        // Get today's date and time
        var now = new Date().getTime();

        // Find the distance between now and the count down date
        var distance = countDownDate - now;

        // Time calculations for days, hours, minutes and seconds
        var days = Math.floor(distance / (1000 * 60 * 60 * 24));
        var hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
        var minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
        var seconds = Math.floor((distance % (1000 * 60)) / 1000);

        // Display the result in the element with id="demo"
        document.getElementById("demo").innerHTML = days + "d " + hours + "h "
        + minutes + "m " + seconds + "s ";

        // If the count down is finished, write some text
        if (distance < 0) {
            clearInterval(x);
            document.getElementById("demo").innerHTML = "EXPIRED";
        }
        }, 1000);
    }

    /** main scheduler
     * 
     */
    function setupScheduler() {
        console.log("setting up scheduler")
        if(self.scheduler!=null){
            clearInterval(self.scheduler)
        }

        let delay = 0;
        if(self.nextDeadline!=null && self.nextDeadline!="None"){
            let localTime = new Date(self.nextDeadline).toLocaleString()
            let curTime = new Date();
            delay = Math.abs(new Date(localTime) - curTime) // TODO: - ( minus the three minutes being added somehow);
            console.log(delay);
            timer(localTime)
        }
        console.info(delay)

        self.scheduler = setInterval(function () {
            if(self.callInProgress!=true){
                self.callInProgress = true
                executionHandler()
            }
        }, delay)
        
    }
    
  return self;
}(window, App || {}))

App.init("13568");