var motion;
var startTime = 0;
// TODO: CHECK IF MOBILE!!
var isMobile = false,
        useDeviceMotion = false;

var scopedCheckQueue;

var ua = navigator.userAgent,
        isIe = ua.indexOf('MSIE') > -1 || ua.indexOf('Trident') > -1;
var useLockedControls = true,
        controls = useLockedControls ? ERNO.Locked : ERNO.Freeform;

window.cube = new ERNO.Cube({
    hideInvisibleFaces: isMobile,
    controls: controls,
    renderer: isIe ? ERNO.renderers.IeCSS3D : null
});

cube.hide();

var container = document.getElementById('cube_container');
container.appendChild(cube.domElement);

if (isMobile) {
    document.body.classList.add('mobile');
}
cube.addEventListener('click', function(evt) {
    if (!cube.mouseControlsEnabled) {
        return;
    }
    var cubelet = evt.detail.cubelet,
            face = cubelet[evt.detail.face.toLowerCase()],
            axis = new THREE.Vector3(),
            exclude = new THREE.Vector3(1, 0, 0),
            UP = new THREE.Vector3(0, 1, 0),
            normal = ERNO.Direction.getDirectionByName(face.normal).normal.clone(),
            slice;
    normal.x = Math.abs(normal.x);
    normal.y = Math.abs(normal.y);
    normal.z = Math.abs(normal.z);
    var l = cube.slices.length;
    while (l-- > 0) {
        slice = cube.slices[l];
        axis.copy(slice.axis);
        axis.x = Math.abs(axis.x);
        axis.y = Math.abs(axis.y);
        axis.z = Math.abs(axis.z);
        if (slice.cubelets.indexOf(cubelet) !== -1 &&
                axis.equals(UP)) {
            break;
        }
    }
    var command = slice.name.substring(0, 1);
    if (slice === cube.down) command = command.invert();
    cube.twist(command);
});
if (controls === ERNO.Locked) {
    var fixedOrientation = new THREE.Euler(Math.PI * 0.1, Math.PI * -0.25, 0);
    cube.object3D.lookAt(cube.camera.position);
    cube.rotation.x += fixedOrientation.x;
    cube.rotation.y += fixedOrientation.y;
    cube.rotation.z += fixedOrientation.z;
}
cube.camera.position.z = 1600;
cube.camera.fov = 30;
cube.camera.updateProjectionMatrix();
if (isMobile) {
    cube.position.y = 0;
    cube.position.z = 0;
} else {
    cube.position.y = 0;
    cube.position.z = 0;
}
cube.mouseControlsEnabled = false;
cube.keyboardControlsEnabled = false;
cube.twistCountDown = 0;
cube.audioList = [];
cube.audio = 0;
ERNO.RED.hex = '#DC422F';
ERNO.WHITE.hex = '#FFF';
ERNO.BLUE.hex = '#3D81F6';
ERNO.GREEN.hex = '#009D54';
ERNO.ORANGE.hex = '#FF6C00';
ERNO.YELLOW.hex = '#FDCC09';
ERNO.COLORLESS.hex = '#000000';

var Plane = function(cube, name, className) {
    THREE.Object3D.call(this);
    cube.object3D.add(this);
    this.domElement = document.createElement('div');
    this.domElement.classList.add(className);
    this.domElement.id = name;
    this.css3DObject = new THREE.CSS3DObject(this.domElement);
    this.css3DObject.name = 'css3DObject-' + name;
    this.add(this.css3DObject);
};
Plane.prototype = Object.create(THREE.Object3D.prototype);
if (!isIe) {
    var shadow = new Plane(cube, 'shadow', 'shadow');
    shadow.rotation.set(
            (90).degreesToRadians(),
            (0).degreesToRadians(),
            (0).degreesToRadians()
    );
    shadow.position.y = -300;
    function updateShadow() {
        requestAnimationFrame(updateShadow);
        shadow.rotation.z = cube.slicesDictionary['y'].rotation;
    }
    requestAnimationFrame(updateShadow);
}
window.setTimeout(setupLogo, 100);

function setupLogo() {
    cube.rotation.set(
            (25).degreesToRadians(),
            (-45).degreesToRadians(),
            (0).degreesToRadians()
    );
    cube.typeCubeletIds = new Array(8, 17, 16, 23, 20, 12, 21, 25);
    cube.typeCubelets = new ERNO.Group();
    cube.cubelets.forEach(function(cubelet, index) {
        cube.typeCubeletIds.forEach(function(id) {
            if (cubelet.id == id) {
                cube.typeCubelets.add(cubelet);
                cubelet.logo = true;
            }
        });
    });

    var stickerLogo = document.getElementsByClassName('stickerLogo');
    if (stickerLogo.length > 0) {
        stickerLogo[0].classList.remove('stickerLogo');
    }

    cube.twistDuration = 0;
    var LOGO_SEQUENCE = 'zzx';
    cube.twistCountDown = LOGO_SEQUENCE.length;
    scopedCheckQueue = checkQueue.bind(this, startScrambleAnimation);
    cube.addEventListener('onTwistComplete', scopedCheckQueue);
    cube.twist(LOGO_SEQUENCE);

    setTimeout(scrambleCube, 1000);
}
function enableDeviceMotion() {
    if (!motion) {
        motion = deviceMotion(cube, container);
        motion.decay = 0.1;
        motion.range.x = Math.PI * 0.02;
        motion.range.y = Math.PI * 0.02;
        motion.range.z = 0;
    }
    motion.paused = false;
}
function pauseDeviceMotion() {
    motion.paused = true;
}

// TODO: MAIN FUNCTION CUBE START
function startInteractiveCube() {
    if (!isMobile && useDeviceMotion) {
        enableDeviceMotion();
    }
    // var sentCertificate = false;
    startTime = (new Date()).getTime();
    cube.twistDuration = 500;
    cube.moveCounter = 0;
    cube.addEventListener('onTwistComplete', function(e) {
        if (cube.undoing) {
            cube.moveCounter++;
        }
        if (cube.isSolved()) {
            // TODO: USE CUBE IS SOLVED
        }
    });
    cube.mouseControlsEnabled = true;
    cube.keyboardControlsEnabled = true;
}

function checkQueue(callback) {
    if (cube.twistQueue.history.length - cube.twistCountDown == 0) {
        cube.removeEventListener('onTwistComplete', scopedCheckQueue);
        callback();
    }
}

function startScrambleAnimation() {
    cube.show();
}

function scrambleCube() {

    // Animates position from start to 0 0 0
    // new TWEEN.Tween(cube.position)
    // .to({
    //             x: 0,
    //             y: 0,
    //             z: 0
    //         }, 3000)
    // .easing(TWEEN.Easing.Quartic.InOut)
    // .start(cube.time);

    // rotates cube
    new TWEEN.Tween(cube.rotation)
    .to({
                x: (25).degreesToRadians(),
                y: (-42).degreesToRadians(),
                z: 0
            }, 3000)
    .easing(TWEEN.Easing.Quartic.InOut)
    .start(cube.time);

    //TO DO: USE FOR SHUFFLE
    cube.twistDuration = 240;
    var WCA_SCRAMBLE_SHORT = 'sseemm';
    cube.twistCountDown =
            WCA_SCRAMBLE_SHORT.length + cube.twistQueue.history.length;
    cube.twist(WCA_SCRAMBLE_SHORT);

    scopedCheckQueue = checkQueue.bind(this, startInteractiveCube);
    cube.addEventListener('onTwistComplete', scopedCheckQueue);
}

var _queryResult = "";

$("#performQueryResult").click(function () {
    if (_queryResult.length > 0) {
        $("#performQueryResult").prop("disabled", true);
        cube.twist(_queryResult);
        _queryResult = "";
        $("#numberOfMoves").text("-");
    }
    return;
});

$("#sendToSolver").click(function () {
    function turnRight(array) {
        if (!array)
            return [];

        var resultingArray = [];
        var N = Math.sqrt(array.length);

        for (var j = 0; j < N; j++)
            for (var i = N - 1; i >= 0; i--)
                resultingArray.push(array[i * N + j]);

        return resultingArray;
    }
    function turnLeft(array) {
        if (!array)
            return [];

        var resultingArray = [];
        var n = Math.sqrt(array.length);

        for (var j = n - 1; j >= 0; j--)
            for (var i = 0; i < n; i++)
                resultingArray.push(array[i * n + j]);

        return resultingArray;
    }

    _queryResult = "";

    var sides = [];

    sides.push({
        direction: "u",
        cells: window.cube.up.getColors(ERNO.Direction.UP)
    });

    sides.push({
        direction: "b",
        cells: turnRight(window.cube.back.getColors(ERNO.Direction.BACK))    // Turn RIGHT
    });

    sides.push({
        direction: "f",
        cells: window.cube.front.getColors(ERNO.Direction.FRONT)
    });

    sides.push({
        direction: "l",
        cells: window.cube.left.getColors(ERNO.Direction.LEFT)
    });

    sides.push({
        direction: "r",
        cells: turnLeft(window.cube.right.getColors(ERNO.Direction.RIGHT))    // Turn LEFT
    });

    sides.push({
        direction: "d",
        cells: turnRight(window.cube.down.getColors(ERNO.Direction.DOWN))    // Turn RIGHT
    });

    var uri = "/api/Cube/Cross/";

    $("#sendToSolver").prop("disabled", true);
    $("#performQueryResult").prop("disabled", true);

    $.post(uri,
    { '': JSON.stringify(sides) },
    function (data) {
        _queryResult = data;
        $("#numberOfMoves").text(data.length);
        //console.log(data);

        var fl = data.length;
        var l = fl / 4;

        if (l > 1) {
            var resultingString = "";
            var index = 0;

            while (index < fl) {
                resultingString += data.substring(index, (index + 4) > (fl - 1) ? (fl) : (index + 4)) + " ";
                index += 4;
            }

            $("#queryResult").text(resultingString);
        } else {
            $("#queryResult").text(data);
        }

        $("#sendToSolver").prop("disabled", false);
        $("#performQueryResult").prop("disabled", false);
    });
    return;
});

