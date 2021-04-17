module.exports = class Server {
    constructor(isLocal = false) {
        let server = this;    
    }

    //Interval update every 100 miliseconds
    onUpdate() {
        
    }

    //Handle a new connection to the server
    onConnected(socket) {
        console.log("client connected")
        
        // TODO send position of other tanks to that user
        // TODO send position of this tank to this user 
    }

    handleMessage(socket, data){
        let parsed = JSON.parse(data);

        if(parsed.method == 'position')
        {
            console.log(JSON.parse(parsed.message))
        }
        
        // TODO for each user send position
        socket.send(JSON.stringify({method: "position", message: JSON.stringify({x: 112, y: 122, tankId: 1})}))
        
    }

    //Handle a new connection to the server
    onDisconnected(socket) {
        console.log("client disconnected")
    }
}