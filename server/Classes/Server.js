let Client = require('./Client.js')

var alreadyUsedX = [];
var alreadyUsedZ = [];
var xList = [13, 6, 3, -3, 8];
var zList = [-34, 18, 9, 2, -15];
var positionsNo = xList.length

module.exports = class Server {
    constructor(wss, isLocal = false) {
        let server = this;
        this.clients = []; // {socket: {id:, activeX:, activeY}}
        this.websocketServer = wss;
    }

    connectNewClient(socket, id) {
        var spawnCords = this.generateSpawn()
    

        // send tank id to connected user
        socket.send(JSON.stringify({
            type: "first",  
            message: JSON.stringify(
                {
                    id_given: id
                })
        }))

        
        // send position of all clients to new connected user
        this.clients.forEach((client)=>{
            socket.send(JSON.stringify({
                type: "init",  
                message: JSON.stringify(
                {
                    x: client.activeX, 
                    y: client.activeZ, 
                    tankId: client.id
                })
            }))
        })

        let client = new Client(socket, spawnCords.x, spawnCords.z, id);
        this.clients.push(client)

        socket.send(JSON.stringify({
            type: "init",  
            message: JSON.stringify(
            {
                x: spawnCords.x, 
                y: spawnCords.z, 
                tankId: id
            })
        }))

        return client
    }

    disconnectClient(socket)
    {
        for(var i=0;i<this.clients.length; i++)
        {
            if(this.clients[i].socket == socket)
            {
                this.clients.splice(i,1)
                break
            }
        }
    }


    //Interval update every 100 miliseconds
    onUpdate() {
        
    }

    //Handle a new connection to the server
    onConnected(socket) {
        console.log("client connected")
        
        // socket.send(JSON.stringify({
        //     type: "position", 
        //     message: JSON.stringify({x: 112, y: 122, tankId: 1})
        // }))
        // TODO send position of other tanks to that user
        // TODO send position of this tank to other users
    }

    validateMessage(id)
    {
        // TODO token validation of message
        return true;
    }

    handleMessage(socket, data){
        let parsed = JSON.parse(data);
        console.log('parsed', parsed)

        // tankId = parsed.id
        if(!this.validateMessage(parsed.id))
            return;

        if(parsed.type == 'position')
        {
            console.log(JSON.parse(parsed.message))
        }

        if(parsed.type == 'fire')
        {
            console.log(JSON.parse(parsed.message))
        }

        if(parsed.type == 'movement')
        {
            console.log(JSON.parse(parsed.message))
            console.log(data)
            this.websocketServer.broadcast(data, socket);
        }
        
    }

    //Handle a new connection to the server
    onDisconnected(socket) {
        console.log("client disconnected")
    }

    generateSpawn(){
        if(alreadyUsedX.length == 0){
            var index = Math.floor(Math.random() * positionsNo);
            var x = xList[index];
            index = Math.floor(Math.random() * positionsNo);
            var z = zList[index];
            alreadyUsedX.push(x);
            alreadyUsedZ.push(z);
        }else{
            while(true){
                var index = Math.floor(Math.random() * positionsNo);
                var x = xList[index];
                index = Math.floor(Math.random() * positionsNo);
                var z = zList[index];
            
                if(alreadyUsedX.includes(x) || alreadyUsedZ.includes(z)){
                    continue;
                }else{
                    alreadyUsedX.push(x);
                    alreadyUsedZ.push(z);
                    break;
                }
            }
        }
        var value = {
            x: x,
            z: z,
          };
    
        return value;
    }
}