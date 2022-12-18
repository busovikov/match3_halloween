mergeInto(LibraryManager.library, {

  InitPlayer: function (){
    initializePlayer();
  },

  ShowFullScreenAdv: function () {
    console.log("ShowFullScreenAdv");
    adv = ysdk.adv.showFullscreenAdv({
      callbacks: {
        onClose: function (wasShown) {
          myGameInstance.SendMessage('Advert', 'OnAdvertComplete');
        },
        onError: function (error) {
          myGameInstance.SendMessage('Advert', 'OnAdvertComplete');
        }
      }
    })
    console.log("adv obj: ",adv);
  },

  GetNoAuth: function (){
    return player.getMode() === 'lite';
  },

  GetLang: function () {
    var returnStr = ysdk.environment.i18n.lang;
    var bufferSize = lengthBytesUTF8(returnStr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(returnStr, buffer, bufferSize);
    return buffer;
   },
  
  CanReview: function () {
    ysdk.feedback.canReview().then(({ value, reason }) => {
      myGameInstance.SendMessage('Advert', 'OnCanReview', value ? 1 : 0);
      console.log("canReview: ", value ? 1 : 0, reason);
      });
  },

  RateGame: function () {
    console.log("try to rate");
    ysdk.feedback.canReview()
      .then(({ value, reason }) => {
        console.log("canReview: ", value ? 1 : 0, reason);
        if (value) {
          ysdk.feedback.requestReview()
            .then(({ feedbackSent }) => {
              myGameInstance.SendMessage('Advert', 'OnAdvertComplete');
              console.log(feedbackSent);
            })
        } else {
          myGameInstance.SendMessage('Advert', 'OnAdvertComplete');
        }
      })
  },

  AuthorizePlayer: function () {
    auth();
  },

  SaveData: function (object) {
    var data = UTF8ToString(object);
    var obj = JSON.parse(data);
    player.setData(obj, true).then(() => {
    });
  },

  LoadData: function () {
    LoadPlayerData();
  },

  SetScoreToLeaderBoard: function (val) {
    ysdk.isAvailableMethod('leaderboards.setLeaderboardScore').then(res => {
      if (res)
      ysdk.getLeaderboards().then(lb => {lb.setLeaderboardScore('Score', val)});
      else
        console.log("leadebord unavailable: ", val);
    });
  },

  SetBestTimeToLeaderBoard: function (val) {
    ysdk.isAvailableMethod('leaderboards.setLeaderboardScore').then(res => {
      if (res)
      ysdk.getLeaderboards().then(lb => {lb.setLeaderboardScore('BestTime', val)});
      else
        console.log("leadebord unavailable: ", val);
    });
  },

  SetBestMovesToLeaderBoard: function (val) {
    ysdk.isAvailableMethod('leaderboards.setLeaderboardScore').then(res => {
      if (res)
        ysdk.getLeaderboards().then(lb => {lb.setLeaderboardScore('BestMoves', val)});
      else
        console.log("leadebord unavailable: ", val);
    });
  },

  ConsumePurchase: function (val){
    var data = UTF8ToString(val);
    console.log("consumePurchase: ", data);
    payments.consumePurchase(data);
  },

  BuyCoins: function (val) {
    payments.getPurchases().then(purchases =>{
       console.log("found some", purchases);
       purchases.forEach(HandleOldPurchase);
    });
    console.log("BuyCoins ", payments);
    if (val == 0) {
      payments.purchase({ id: 'coins5' }).then(purchase => {
        console.log("purchase 5 success");
        SendPurchase(5, purchase.purchaseToken);
      }).catch(err => { console.log("buy coins fail"); });
    }
    else if (val == 1) {
      payments.purchase({ id: 'coins20' }).then(purchase => {
        console.log("purchase 20 success");
        SendPurchase(20, purchase.purchaseToken);
      }).catch(err => { console.log("buy coins fail"); });
    }
    else if (val == 2) {
      payments.purchase({ id: 'coins100' }).then(purchase => {
        console.log("purchase 100 success");
        SendPurchase(100, purchase.purchaseToken);
      }).catch(err => { console.log("buy coins fail"); });
    }
  },

});