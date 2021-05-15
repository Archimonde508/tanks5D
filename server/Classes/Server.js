let Client = require('./Client.js')


var alreadyUsed = [];
var spawnPositions = [
    [0, 0], [0, 10], [0, 15], [0, -15], [0, -23],
    [10, -32], [11, -11], [12, 10], [-7, 5]
    // -8, 26 jest bledne
];
var currentPlayers = 0;
var gameFinishedList = [];
var positionsNo = spawnPositions.length

module.exports = class Server {
    constructor(wss, isLocal = false) {
        let server = this;
        this.clients = []; // {socket: {id:, activeX:, activeY}}
        this.websocketServer = wss;
    }


    generateFreeId()
    {
        for(var i = 0; i < 3; i++)
        {
            var free = true
            this.clients.forEach((client)=>{
                if(client.id == i)
                    free = false
            })

            if(free)
                return i
        }
        return 0
    }

    connectNewClient(socket) {
        var spawnCords = this.generateSpawn()

        console.log(spawnCords);

        var id = this.generateFreeId()
        gameFinishedList.push(false);

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
                    x: client.x, 
                    y: client.z, 
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

    zacznijGre(socket){
        console.log("nanan zaczynamy gre");

        this.clients.forEach((client)=>{
            console.log("2 razy powinno sie pojawic");
            client.socket.send(JSON.stringify({
                type: "finish",  // new round
                message: JSON.stringify(
                {
                    id: client.id,
                    context: "start"
                })
            }))
        })
    }

    incrementCurrentplayersBecuaseJsIsShit(){
        currentPlayers++;
        console.log(currentPlayers);
    }

    getCurrentPlayers(){
        return currentPlayers;
    }

    startGame(){
        gameFinishedList = new Array(currentPlayers);
        for(var i = 0; i < currentPlayers; i++){
            gameFinishedList[i] = false;
        }
    }

    disconnectClient(socket)
    {
        for(var i=0;i<this.clients.length; i++)
        {
            // console.log(socket)
            if(this.clients[i].socket == socket)
            {
                console.log("Client removed")
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
        function sleep(ms) {
            return new Promise(resolve => setTimeout(resolve, ms));
        }

        let parsed = JSON.parse(data);
        //console.log('parsed', parsed)

        // tankId = parsed.id
        if(!this.validateMessage(parsed.id))
            return;

        if(parsed.type == 'position')
        {
            //console.log(JSON.parse(parsed.message))
            this.websocketServer.broadcast(data, socket);
        }

        if(parsed.type == 'fire')
        {
            //console.log(JSON.parse(parsed.message))
            this.websocketServer.broadcast(data, socket);
        }

        if(parsed.type == 'movement')
        {
            //console.log(JSON.parse(parsed.message))
            this.websocketServer.broadcast(data, socket);
        }

        if(parsed.type == 'finish'){   
            parsed.message = JSON.parse(parsed.message)

            var id = parsed.message.id; //read from json

            gameFinishedList[id] = true;
            console.log(this.allPlayersFinished());
            if(this.allPlayersFinished()){
                for(var i = 0; i < currentPlayers; i++){
                    gameFinishedList[i] = false;
                }
                
                console.log("Zaraz kolejna runda");
                sleep(1000).then(() => { this.startNewRound()});

            }
        }        
    }

    startNewRound(){
        this.clients.forEach((client)=>{
            var spawnCords = this.generateSpawn();
            console.log("new round starts");
            var toSend = JSON.stringify({
                type: "newRound",  
                message: JSON.stringify(
                {
                    x: spawnCords.x, 
                    y: spawnCords.z, 
                    tankId: client.id
                })
            })
            client.socket.send(toSend)

            //broadcast wylosowanej pozycji 
            this.websocketServer.broadcast(toSend, client.socket);
        })


        
    }

    allPlayersFinished(){
        for(var i = 0; i < currentPlayers; i++){
            if(gameFinishedList[i] == false){
                return false;
            }
        }
        return true;
    }

    generateSpawn(){
        if(alreadyUsed.length == 0){
            var index = Math.floor(Math.random() * positionsNo);
            var spawnPos = spawnPositions[index];
            alreadyUsed.push(spawnPos);
        }else{
            while(true){
                var index = Math.floor(Math.random() * positionsNo);
                var spawnPos = spawnPositions[index];
            
                if(alreadyUsed.includes(spawnPos)){
                    continue;
                }else{
                    alreadyUsed.push(spawnPos);
                    break;
                }
            }
        }
    
        var returned = {
            x: spawnPos[0],
            z: spawnPos[1]
        }

        return returned;
    }

}