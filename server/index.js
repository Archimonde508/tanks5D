let Server = require('./Classes/Server')
let server = new Server(process.env.PORT == undefined);

var currentPlayers = 0;
var alreadyUsedX = [];
var alreadyUsedZ = [];
var xList = [13, 6, 3, -3, 8];
var zList = [-34, 18, 9, 2, -15];
var positionsNo = xList.length

const WebSocket = require('ws')
const wss = new WebSocket.Server({ port: 3000 },()=>{
    console.log('server started')
})
wss.on('connection', (ws) => {
    let connection = server.onConnected(ws);

    
    var spawnCords = generateSpawn();

    ws.send(JSON.stringify({
        type: "init",  
        message: JSON.stringify(
            {
                x: spawnCords.x, 
                y: spawnCords.z, 
                tankId: currentPlayers
            })
    }))
    console.log("haha");
    currentPlayers++;
    
    ws.on('message', (data) => {
        server.handleMessage(ws, data);
        console.log('data received \n %o', data)
    })

    ws.on('listening',()=>{
        console.log('listening on 3000')
    })

    ws.on('disconnect', (ws)=>{
        console.log('disconnected');
    }) 
    
})

function generateSpawn(){
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
