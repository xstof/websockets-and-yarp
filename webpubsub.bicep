param location string = resourceGroup().location
param webpubsub_name string = 'xstof-webpubsub'
param webpubsub_hubname string = 'xstofhub'
// param webpubsub_eventhandler_urltemplate string


resource webpubsub 'Microsoft.SignalRService/webPubSub@2021-10-01'= {
  name: webpubsub_name
  location: location
  sku: {
    name: 'Standard_S1'
    capacity: 1
    tier: 'Standard'
  }
  resource xstofhub 'hubs' = {
    name: webpubsub_hubname
    properties: {
      eventHandlers: [
        // {
        //   urlTemplate: webpubsub_eventhandler_urltemplate
        // }
      ]
    }
  }
}
