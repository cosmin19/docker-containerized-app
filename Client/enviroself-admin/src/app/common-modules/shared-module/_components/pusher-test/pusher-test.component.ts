import { Component, OnInit } from '@angular/core';
import { PusherService } from '../../_services/pusher.service';
import { PusherConstants } from '../../_constants/pusher-constants';

export class NewLikeDto {
    likes: number;
    constructor(likes: number) {
        this.likes = likes;
    }
}
export class NewLikeResponseDto {
    likes: number;
}
export class UserStatusDto {
    id: number;
    online: boolean;
    email: string;
}

@Component({
    templateUrl: './pusher-test.component.html'
})
export class PusherTestComponent implements OnInit {
    title = 'Pusher Liker';
    likes: any = 10;

    constructor(private _pusherService: PusherService) { }

    ngOnInit() {
        this._pusherService.channel.bind(PusherConstants.LIKE_EVENT_NAME, (data: NewLikeResponseDto) => {
            this.likes = data.likes;
        });
    }

    liked() {
        this.likes = parseInt(this.likes, 10) + 1;

        let model = new NewLikeDto(this.likes);

        this._pusherService.like(model).subscribe(() => {});
    }

}
