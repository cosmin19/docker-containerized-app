import { Injectable } from '@angular/core';

@Injectable()
export class AppConfig {

    pusherKey: string = 'e514b8a25b3df111064b';
    pusherCluster: string = 'eu';

    facebookClientId: string = "2248823325349811";
    
    baseUrl:string = 'http://localhost:50000/';
    _cdnBaseUrl:string = 'http://localhost:50001/';

}