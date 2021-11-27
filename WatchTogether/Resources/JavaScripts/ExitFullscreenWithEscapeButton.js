console.log('Exit fullscreen helper script execution started!');

function logKey(e) {
	//console.log(`${e.key=='Escape'} | ${document.fullscreen} | ${document.fullscreenEnabled} | ${document.webkitIsFullScreen}`);
	if (e.key == 'Escape' && document.webkitIsFullScreen == true) {
		document.webkitExitFullscreen();
	}
}

document.addEventListener('keydown', logKey);