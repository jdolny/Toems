function FineUploader(myToken, myBaseUrl, moduleGuid, moduleId, moduleType) {
            var uploader = new qq.FineUploader({
                debug: true,
                element: document.getElementById('uploader'),
                request: {
                    endpoint: myBaseUrl + '/Upload/UploadFile',
                    customHeaders: {
                        "Authorization": "Bearer " + myToken
                    },
                    params: {
                        uploadMethod: "standard",
                        moduleGuid: moduleGuid,
                        moduleId: moduleId,
                        moduleType: moduleType 
                    }
                },

                retry: {
                    autoAttemptDelay: 2,
                    enableAuto: true,
                    maxAutoAttempts: 10
                },
                chunking: {
                    enabled: true,
                    partSize: 4096000,
                    success: {
                        endpoint: myBaseUrl + '/Upload/ChunkingComplete'
                    }
                },
                thumbnails: {
                    placeholders: {

                    }
                },
                validation: {

                    itemLimit: 100

                },
                autoUpload: false,
                callbacks: {
                    onAllComplete: function (succeeded, failed) {
                        if (succeeded.length > 0) {
                            DotNet.invokeMethodAsync("ToemsUI.Client", "OnUploadSuccess");
                        }
                    }
                }
            });

            qq(document.getElementById("trigger-upload")).attach("click", function () {
                uploader.uploadStoredFiles();
            });
        }
