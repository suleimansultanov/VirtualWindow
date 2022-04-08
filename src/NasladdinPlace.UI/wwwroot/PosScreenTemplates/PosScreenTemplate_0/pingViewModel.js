 function PingViewModel() {
  var self = this;
  const servers = {"global": 'https://google.com/favicon.ico',
    "ru": 'https://yandex.ru/favicon.ico',
    "hosting": `${AdminUrl}/icons/favicon-16x16.png`,
    "nasladdin": `${ApiUrl}/icons/favicon-16x16.png`};

    const timeout = 'timeout';
    const unchecked = 'unchecked';
    const responded = 'responded';
    const checking = 'checking';
    const error = 'error';
    const nasladdin = 'nasladdin';
    const ru = 'ru';
    const global = 'global';
    const hosting = 'hosting';
  
  var myServers = [];

  Object.keys(servers).forEach(function(index) {
    myServers.push({
      name: servers[index],
      status: ko.observable(unchecked),
      className: index
    });
  });
 
  self.servers = ko.observableArray(myServers);

  self.pingServers = function(){
    Object.keys(self.servers()).forEach(function(index) {
      self.servers()[index].status(checking);
      self.fillRectagle(self.servers()[index].className, 
                      self.servers()[index].status());
      
      new LoadImage(self.servers()[index].name, function (status, e) {
          self.servers()[index].status(`${status}`);
          self.fillRectagle(self.servers()[index].className, 
                      self.servers()[index].status());
      });
    });
  }

  self.areAllServersResponded = function(){
    for (let index = 0; index < self.servers().length; index++) {
      const element = self.servers()[index];
      if(element.status() !== responded){
        return false;
      }
    }
    return true;
  }

  self.isNasladdinAvailable = function(){
    for (let index = 0; index < self.servers().length; index++) {
      const element = self.servers()[index];
      if(element.className === nasladdin){
        if(element.status() === responded)
          return true;
        return false;
      }
    }
  }

  self.iSGlobalAvailable = function(){
    for (let index = 0; index < self.servers().length; index++) {
      const element = self.servers()[index];
      if(element.className === global){
        if(element.status() === responded)
          return true;
        return false;
      }
    }
  }

  self.isRuAvailable = function(){
    for (let index = 0; index < self.servers().length; index++) {
      const element = self.servers()[index];
      if(element.className === ru){
        if(element.status() === responded)
          return true;
        return false;
      }
    }
  }

  self.isHostingAvailable = function(){
    for (let index = 0; index < self.servers().length; index++) {
      const element = self.servers()[index];
      if(element.className === hosting){
        if(element.status() === responded)
          return true;
        return false;
      }
    }
  }

  self.fillRectagle = function(className, status){
    var element = document.getElementsByClassName(className)[0];
    element.classList.remove(timeout);
    element.classList.remove(unchecked);
    element.classList.remove(responded);
    element.classList.remove(checking);
    element.classList.remove(error);
    element.classList.add(status);
  }
};


class LoadImage {
  constructor(ip, callback) {
    if (!this.inUse) {
      this.status = 'unchecked';
      this.inUse = true;
      this.callback = callback;
      this.ip = ip;
      var _that = this;
      this.img = new Image();
      this.img.onload = function () {
        _that.inUse = false;
        _that.callback('responded');
        this.img = null;
      };
      this.img.onerror = function (e) {
        if (_that.inUse) {
          _that.inUse = false; 
          _that.callback('error', e);
          this.img = null;
        }
      };
      this.start = new Date().getTime();
      this.img.src = ip;
      this.timer = setTimeout(function () {
        if (_that.inUse) {
          _that.inUse = false;
          _that.callback('timeout');
          this.img = null;
        }
      }, 1500);
    }
  }
}
