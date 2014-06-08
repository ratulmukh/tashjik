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
									.select("#chordCircle")
									.attr("width", 1250)
									.attr("height", 1250);
								
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
						.select("#chordCircle")
						.attr("width", 1250)
						.attr("height", 1250);
					
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