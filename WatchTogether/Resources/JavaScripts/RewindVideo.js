// Two next lines of code will be presented here when this code will be executed by a command
// var currentTime = 0;
// var paused = false;
var media = document.querySelector('video');

media.currentTime = currentTime;
if (paused == false) {
    if (media.paused == true)
        media.play();
}
else {
    if (media.paused == false)
        media.pause();
}

// Mark the changes of the video playing as 'wasModifiedByScript'
media.wasModifiedByScript = true;

console.log('RewindVideo script was executed successfully!');