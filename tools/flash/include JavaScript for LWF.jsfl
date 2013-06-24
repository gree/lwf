var doc = fl.getDocumentDOM();
var folder = ',js';

var lib;
var items;
var timeline;

var docPath;
var folderPath;
var files;
var directories;

var log = '';
var warningNum = 0;

var includeData;
var type = ['load', 'postLoad', 'enterFrame', 'unload'];
var suffix = ['_load', '_postLoad', '_enterFrame', '_unload'];
var isRoot = false;


init();

function init(){
    if (!doc) {
        alert("[ERROR] Can't open fla file.");
    } else {
        lib = doc.library;
        items = lib.items;

        docPath = doc.pathURI;

        var lastSlash = docPath.lastIndexOf('/');
        folderPath = docPath.substr(0, lastSlash + 1) + folder;

        fl.outputPanel.clear();

        start();
    }
}

function start(){
    trace('----------------------------------------');
    trace('include JavaScript for LWF');
    trace('');

    fl.showIdleMessage(false);

    includeData = [];

    getFiles();
    createFilesData();

    getDirectories();
    createDirectoriesData();

    includeData = createJoinData(includeData);
    includeScriptFile();

    complete();
}

function getFiles(){
    files = FLfile.listFolder(folderPath, 'files');
}

function getDirectories(){
    directories = FLfile.listFolder(folderPath, 'directories');
}

function createFilesData(){
    var lgh = files.length;
    for(var i = 0; i < lgh; i++){
        var obj = createFilesObject(files[i]);
        if(typeof obj !== 'undefined') includeData.push(obj);
    }
}

function createDirectoriesData(){
    var lgh = directories.length;
    for(var i = 0; i < lgh; i++){
        var mcPath = folderPath + '/' + directories[i];
        var frameDirectories = FLfile.listFolder(mcPath, 'directories');
        var lgh2 = frameDirectories.length;
        for(var j = 0; j < lgh2; j++){
            var framePath = mcPath + '/' + frameDirectories[j];
            var jsFiles = FLfile.listFolder(framePath, 'files');
            var lgh3 = jsFiles.length;
            for(var n = 0; n < lgh3; n++){
                if(jsFiles[n] === '_include.conf'){
                    var directory = [directories[i], frameDirectories[j]].join('/');
                    var conf = getScriptFile(directory + '/_include.conf');
                    conf = deleteNewline(conf);
                    jsFiles = conf.split(',');
                }
            }
            lgh3 = jsFiles.length;
            for(n = 0; n < lgh3; n++){
                var obj = createFilesObject([directories[i], frameDirectories[j], jsFiles[n]]);
                if(typeof obj !== 'undefined') includeData.push(obj);
            }
        }
    }
}

function createFilesObject(param){
    var obj = {};
    if(typeof param === 'string'){
        if(checkFileExtention(param) === false) return undefined;
        var ary = deleteFileExtention(param).split('_');
        obj.fileName = param;
        obj.traceName = [param];
        obj.itemName = ary[0];
        obj.frame = Number(ary[1]);
        obj.type = checkType(ary[2]);
        obj.layerName = 'js';
        if(obj.type !== null) obj.layerName += suffix[obj.type];
        if(typeof ary[3] !== 'undefined'){
            obj.option = ary[3];
        }else{
            obj.option = null;
        }
    }else if(param instanceof Array){
        obj.fileName = param.join('/');
        if(checkFileExtention(obj.fileName) === false) return undefined;
        var frameAndType = param[1].split('_');
        obj.traceName = [obj.fileName];
        obj.itemName = param[0];
        obj.frame = Number(frameAndType[0]);
        obj.type = checkType(frameAndType[1]);
        obj.layerName = 'js';
        if(obj.type !== null) obj.layerName += suffix[obj.type];
        obj.option = deleteFileExtention(param[2]);
    }
    obj.script = getScriptFile(obj.fileName);
    if(obj.script === ''){
        obj.traceName[0] += '\n -> [Warning] not found or empty';
        warningNum++;
    }
    return obj;
}

function deleteFileExtention(str){
    var newStr = str.substr(0, str.lastIndexOf('.'));
    return newStr;
}

function deleteNewline(str){
   var newStr = str.replace(/\r\n/g, "");
   return newStr;
 }

function checkType(str){
    if(typeof str !== 'undefined'){
        var lgh = type.length;
        for(var i = 0; i < lgh; i++){
            if(str === type[i]){
                return i;
            }
        }
    }
    return null;
}

function checkFileExtention(str){
    var bool = false;
    if (!str) {
        return bool;
    }
    var fileTypes = str.split(".");
    var length = fileTypes.length;
    if (length === 0) {
        return bool;
    }
    if(fileTypes[length - 1] === 'js'){
        bool = true;
    }
    return bool;
}

function getScriptFile(fileName){
    var script = FLfile.read(folderPath + '/' + fileName);
    return script;
}

function createJoinData(){
    var data = includeData;
    var newData = [];
    var lgh = data.length;
    for(var i = 0; i < lgh; i++){
        for(var j = 0; j < lgh; j++){
            if(i !== j && data[i].itemName === data[j].itemName && data[i].frame === data[j].frame && data[i].type === data[j].type){
                data[i].script += '\n\n\n\n';
                data[i].script += addScriptHeader(data[j].fileName);
                data[i].script += data[j].script;
                data[i].traceName.push(data[j].traceName[0]);
                data.splice(j, 1);
                j--;
                lgh--;
            }
        }
    }
    newData = data;
    return newData;
}

function addScriptType(type){
    var str = '/* js';
    if(type !== null){
        str += suffix[type];
    }
    str += '\n';
    return str;
}

function addScriptHeader(fileName){
    return '//-----include file:' + fileName + '-----//\n\n';
}

function addScriptFooter(){
    return '\n\n*/';
}

function includeScriptFile(){
    var lgh = items.length;
    for(var i = 0; i < lgh; i++){
      if(items[i].itemType === 'movie clip'){
            var mcPath = items[i].name;
            var mcName = mcPath.substring(mcPath.lastIndexOf('/') + 1);
            var data = searchIncludeData(mcName);
            if(data.length > 0) addData(data, mcPath);
        }
    }
    var rootData = searchIncludeData('root');
    if(rootData.length > 0){
        isRoot = true;
        addData(rootData);
    }
}

function searchIncludeData(mcName){
    var data = [];
    var lgh = includeData.length;
    for(var i = 0; i < lgh; i++){
        if(includeData[i].itemName === mcName){
            data.push(includeData[i]);
        }
    }
    return data;
}

function addData(data, mcPath){
    var lgh = data.length;
    lib.editItem(mcPath);

    trace('');
    trace('{' + data[0].itemName + '}');

    for(var i = 0; i < lgh; i++){
        var layer = addLayer(data[i].layerName);
        addScript(layer, data[i]);

        var lgh2 = data[i].traceName.length;
        for(var j = 0; j < lgh2; j++){
            trace('-> ' + data[i].traceName[j]);
        }
    }
}

function addLayer(layerName){
    timeline = doc.getTimeline();
    var index = timeline.findLayerIndex(layerName);
    var layer;

    if(typeof index === 'undefined'){
      var newIndex = timeline.addNewLayer(layerName);
      layer = timeline.layers[newIndex];
      timeline.setSelectedLayers(newIndex);
    }else{
      layer = timeline.layers[index[0]];
      timeline.setSelectedLayers(index[0]);
      timeline.currentLayer = index[0];
      timeline.setSelectedFrames(0, 0);
    }
    return layer;
}

function addScript(layer, data){
    var maxFrame = layer.frames.length - 1;

    if(maxFrame < data.frame){
        timeline.currentFrame = maxFrame;
        timeline.insertFrames(data.frame - maxFrame);
    }

    var addFrame = layer.frames[data.frame];
    if(data.frame != addFrame.startFrame){
        timeline.convertToBlankKeyframes(data.frame);
    }
    data.script = addScriptType(data.type) + addScriptHeader(data.fileName) + data.script + addScriptFooter();
    layer.frames[data.frame].actionScript = data.script;
}

function complete(){
    fl.showIdleMessage(true);

    if(isRoot === false) lib.editItem('root');

    doc.save();
    releaseLog();
}

function trace(msg){
    if(msg === ''){
        log += '\n';
    }else{
        log += msg;
        log += '\n';
    }
}

function releaseLog(){
    if(warningNum > 0){
        fl.trace('Please check');
        if(warningNum === 1){
            fl.trace('Warning ' + warningNum + ' file');
        }else{
            fl.trace('Warning ' + warningNum + ' files');
        }
    }else{
        fl.trace('Congratulations! No error!');
    }

    fl.trace('');
    fl.trace(log);
}