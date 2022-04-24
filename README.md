Løsningen består af 3 programmer:  
ElprisData, ElprisProducer og ElprisRequester.  
Der skal køre en Kafka på serveren Ubuntu2004.

Der skal være oprettet 4 topics:  
requesteldata  
publisheldata  
&  
requestelpris  
publicelpris  

ElprisData subscriber på topic "requesteldata".  
Hvis der kommer en request svares der tilbage via topic "publisheldata".

ElprisProducer subscriber på topic "requestelpris".  
Hvis der kommer en request svares der tilbage via topic "publicelpris".  
ElprisProducer requester data via topic "requesteldata" og forventer svar via topic "publisheldata".  

ElprisRequester requester elpris via topic requestelpris og forventer svar via topic publicelpris.

ElprisRequester -> ElprisProducer -> ElprisData  
&  
ElprisData -> ElprisProducer -> ElprisRequester 

Screenshots fra de 3 programmer:
![Screenshots4CaseDel6](https://user-images.githubusercontent.com/80167961/164992287-bebdf44f-d5be-415d-b8a9-addf58769b64.png)
