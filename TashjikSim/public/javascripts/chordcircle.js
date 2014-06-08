var wsUri = "ws://localhost:9000/websocket"; 
			var output; 
			var websocket;
			var circleData = [];
			var lineData = [];
		
				
			window.onload = function(){ 
				output = document.getElementById("output"); 
				
				var form = document.getElementById('simConfigForm');
				var startButton = document.getElementById('startButton');
				var cancelButton = document.getElementById('cancelButton');
				var nodeCountField = document.getElementById('nodeCount');
				var dataObjectsCountField = document.getElementById('dataObjectsCount');
				
				setupWebSocket();

				startButton.onclick = function(evt) { onStartButtonClick(evt) };
				cancelButton.onclick = function(evt) { onCancelButtonClick(evt) };
				  
				
			};
			
			function onStartButtonClick(e) { 
				e.preventDefault();

				  var nodeCount = document.getElementById('nodeCount').value;
				  var dataObjectsCount =document.getElementById('dataObjectsCount').value;
				  var submitData = "{\"msgType\" : \"startSim\", \"nodeCount\" : " + nodeCount + ", \"dataObjectsCount\" : " + dataObjectsCount + "}";	
				  doSend(submitData);

				  return false;
			}
			
			function onCancelButtonClick(e) { 
				e.preventDefault();
    			var submitData = "{\"msgType\" : \"cancelSim\"}";	
				doSend(submitData);

				  return false;
			}
			

     		function setupWebSocket() { 
				websocket = new WebSocket(wsUri); 
				websocket.onopen = function(evt) { onWebSocketOpen(evt) };
				websocket.onclose = function(evt) { onWebSocketClose(evt) };
				websocket.onmessage = function(evt) { onWebSocketMessage(evt) };
				websocket.onerror = function(evt) { onWebSocketError(evt) };
			}
			
			function onWebSocketOpen(evt) { 
				writeToScreen("CONNECTED"); 
				
			}
			
			function onWebSocketClose(evt) {
				writeToScreen("DISCONNECTED");
			}
			
			function onWebSocketMessage(evt) { 
				writeToScreen('<span style="color: blue;">RESPONSE: ' + evt.data+'</span>'); 

				if((JSON.parse(evt.data)).SVGType == "circle")
				{	
								circleData.push(JSON.parse(evt.data));
				
								var svg = d3.select("body")
									.select("#chordCircle");
								
								var enter = svg.selectAll("circle")
									.data(circleData)
									.enter().append("circle");
									
									enter.attr("cx", function(d) { return d.cx; })
									.attr("cy", function(d) { return d.cy; })
									.attr("r", 3)
									.attr("fill", "red");

				}
				else if((JSON.parse(evt.data)).SVGType == "line")
				{	
					lineData.push(JSON.parse(evt.data));
	
					var svg = d3.select("body")
						.select("#chordCircle");
					
					var enter = svg.selectAll("line")
						.data(lineData)
						.enter().append("line");
						
						enter.attr("x1", function(d) { return d.x1; })
						.attr("y1", function(d) { return d.y1; })
						.attr("x2", function(d) { return d.x2; })
						.attr("y2", function(d) { return d.y2; })
						.attr("style", "stroke:rgb(22, 116, 50);stroke-width:2");
						//.attr("fill", "black");

				}	
				else
				{
					hopData = JSON.parse(evt.data);
					//hopData = [{"SVGType":"HopCount","letter":"1","frequency":2},{"SVGType":"HopCount","letter":"2","frequency":10}];
					
					var svg = d3.select("body").select("#chordCircle");

					
					
					var hopData11 = [{"SVGType":"HopCount","letter":"1","frequency":5},{"SVGType":"HopCount","letter":"2","frequency":1}, {"SVGType":"HopCount","letter":"3","frequency":1},{"SVGType":"HopCount","letter":"4","frequency":1}, {"SVGType":"HopCount","letter":"5","frequency":1},{"SVGType":"HopCount","letter":"6","frequency":1}, {"SVGType":"HopCount","letter":"7","frequency":1},{"SVGType":"HopCount","letter":"8","frequency":1}, {"SVGType":"HopCount","letter":"9","frequency":1},{"SVGType":"HopCount","letter":"10","frequency":1}, {"SVGType":"HopCount","letter":"11","frequency":1},{"SVGType":"HopCount","letter":"12","frequency":1}, {"SVGType":"HopCount","letter":"13","frequency":1},{"SVGType":"HopCount","letter":"14","frequency":1}];
					
					x.domain(hopData11.map(function(d) { return d.letter; }));
					y.domain([0, d3.max(hopData11, function(d) { return d.frequency; })]);
					  
					svg.selectAll("rect")
				      .data(hopData)
				      .attr("class", "bar")
				      .attr("x", function(d) { return x(d.letter); })
				      .attr("width", x.rangeBand())
				      .attr("y", function(d) { return y(d.frequency); })
				      .attr("height", function(d) { return height - y(d.frequency); })
				      .attr("fill", "red");
				}
			}
			
			function onWebSocketError(evt) { 
				writeToScreen('<span style="color: red;">ERROR:</span> ' + evt.data); 
			}
			
			function doSend(message) { 
				writeToScreen("SENT: " + message);
				websocket.send(message);
			}
			
			function writeToScreen(message) {
				var pre = document.createElement("p"); 
				pre.style.wordWrap = "break-word";
				pre.innerHTML = message; 
				output.appendChild(pre);
			}
			
			//window.addEventListener("load", init, false); 