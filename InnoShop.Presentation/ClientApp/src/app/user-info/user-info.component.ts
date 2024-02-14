import { Component } from '@angular/core';
import { UserApiProxyService, UserInfoDto } from '../services/user-api-proxy.service';
import { AuthStateProviderService } from '../services/auth-state-provider.service';

@Component({
  selector: 'app-user-info',
  templateUrl: './user-info.component.html',
  styleUrls: ['./user-info.component.css']
})
export class UserInfoComponent {
  isExpanded = false;

  user: UserInfoDto | null = null;

  constructor(proxy: UserApiProxyService, private provider: AuthStateProviderService) {
    proxy.userInfo()
      .subscribe(
        {
          next: (info) => {
            this.user = info;
          }
        }
      );
  }
  
  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  logout() {
    this.provider.removeTokens();
  }
}
