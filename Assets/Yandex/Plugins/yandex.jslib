mergeInto(LibraryManager.library, {

  ShowFullScreenAdv: function () {
    ysdk.adv.showFullscreenAdv({
    callbacks: {
        onClose: function(wasShown) {
          // some action after close
        },
        onError: function(error) {
          // some action on error
        }
    }
})
  },

  RateGame: function () {
    console.log("try to rate");
    ysdk.feedback.canReview()
        .then(({ value, reason }) => {
            if (value) {
                ysdk.feedback.requestReview()
                    .then(({ feedbackSent }) => {
                        console.log(feedbackSent);
                    })
            } else {
                console.log(reason)
            }
        })
  },

  IsPlayerAuthorized: function (){
    var auth = player.getMode() === 'lite';
      console.log("is player Authorized");
      console.log(auth);
      return auth;
  },

  AuthorizePlayer: function (){
    auth();
  },

  GetLang: function () {
    var returnStr = ysdk.environment.i18n.lang;
    var bufferSize = lengthBytesUTF8(returnStr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(returnStr, buffer, bufferSize);
    return buffer;
  },

});