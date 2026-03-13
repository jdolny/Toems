window.updateCountdown = (elementId, time) => {
    const countdownElement = document.getElementById(elementId);
    if (countdownElement) {
        countdownElement.textContent = time;
    }
};

window.getDeviceInfo = () => {
    const userAgent = navigator.userAgent.toLowerCase();
    const isMobile = /android|iphone|ipad|ipod|windows phone|blackberry|mobile|tablet/.test(userAgent);
    return {
        screenWidth: window.innerWidth,
        screenHeight: window.innerHeight,
        isMobile: isMobile,
        isTouchDevice: 'ontouchstart' in window || navigator.maxTouchPoints > 0
    };
};

window.downloadFileWithToken = (url, fileName, token) => {
    fetch(url, {
        headers: {
            'Authorization': `Bearer ${token}`
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }
            return response.blob();
        })
        .then(blob => {
            const link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = fileName;
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            window.URL.revokeObjectURL(link.href);
        })
        .catch(error => console.error('Download failed:', error));
};

function saveAsTextFile(content, fileName) {
    const blob = new Blob([content], { type: 'text/plain' });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = fileName;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    window.URL.revokeObjectURL(url);
}