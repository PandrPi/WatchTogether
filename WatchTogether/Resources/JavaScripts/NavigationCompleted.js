var media = document.querySelector('video');

function PostCommandMessage(commandType, ...parameters) {
    var commandDict = {
        FullTypeName: commandType,
        Parameters: parameters
    };
    window.chrome.webview.postMessage(JSON.stringify(commandDict));
}

function OnPlayEventHandler(e) {
    if (media.wasModifiedByScript == true) {
        media.wasModifiedByScript = false;
        return;
    }

    //console.log('Play at: ' + media.currentTime);
    PostCommandMessage('RewindVideoCommand', media.currentTime, media.paused, 0);
}

function OnPauseEventHandler(e) {
    if (media.wasModifiedByScript == true) {
        media.wasModifiedByScript = false;
        return;
    }

    //console.log('Pause at: ' + media.currentTime);
    PostCommandMessage('RewindVideoCommand', media.currentTime, media.paused, 0);
}

function OnSeeking(e) {
    if (media.wasModifiedByScript == true) {
        media.wasModifiedByScript = false;
        return;
    }

    //console.log('Seeking at: ' + media.currentTime + ' is paused:' + media.paused);
    PostCommandMessage('RewindVideoCommand', media.currentTime, media.paused, 0);
}

media.addEventListener('play', (event) => OnPlayEventHandler(event));
media.addEventListener('pause', (event) => OnPauseEventHandler(event));
media.addEventListener('seeking', (event) => OnSeeking(event));

// This field is used to prevent code from looping. The calling of play and pause methods
// or seeking / assigning a new value for media.currentTime field faires the onplay, onpause
// and onseeking events. So in the RewindVideo script we will set wasModifiedByScript field
// to true value and in this script we will not post any command masseges if it is true
media.wasModifiedByScript = false;

console.log('NavigationCompleted script was executed successfully!');