mergeInto(LibraryManager.library, {

  ShowFullScreenAdv: function () {
    ysdk.adv.showFullscreenAdv({
    callbacks: {
        onClose: function(wasShown) {
          myGameInstance.SendMessage('Advert','OnAdvertComplete');
        },
        onError: function(error) {
          myGameInstance.SendMessage('Advert','OnAdvertComplete');
        },
        onOpen : function(error) {
          console.log("adv is shown");
        },
        onOffline : function(error) {
          myGameInstance.SendMessage('Advert','OnAdvertComplete');
        },
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
      return !auth;
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


  SaveData: function (object) {
    var data = UTF8ToString(object);
    var obj = JSON.parse(data); 
    player.setData(obj,true).then(() => {
    });
  },

  LoadData: function (keys) {
    var data = UTF8ToString(keys);
    var obj = JSON.parse(data); 
      player.getData().then(_data => {
      const ret = JSON.stringify(_data);
      myGameInstance.SendMessage('Config','PlayerStatsReceived', ret);
    });
  },

  SetScoreToLeaderBoard: function (val)
  {
    if (lb != null) lb.setLeaderboardScore('Score', val);
  },

  SetBestTimeToLeaderBoard: function (val)
  {
    if (lb != null) lb.setLeaderboardScore('BestTime', val);
  },

  SetBestMovesToLeaderBoard: function (val)
  {
    if (lb != null) lb.setLeaderboardScore('BestMoves', val);
  },

});