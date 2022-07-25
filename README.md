# IdentityServer.STS.Admin
>SPA implementation from [Skoruba.IdentityServer4.Admin](https://github.com/skoruba/IdentityServer4.Admin)
>includes Identityserver4 and Admin,thanks alot!


### 如何使用
[javascript demo from ids4](https://identityserver4.readthedocs.io/en/latest/quickstarts/4_javascript_client.html)
- 使用以下配置连接到认证demo服务器
```javascript
const config = {
      authority: 'https://isawesome.cn:5000',
      //authority: 'http://localhost:5000',
      client_id: 'spa',
      redirect_uri: 'http://localhost:5501/callback.html',
      post_logout_redirect_uri: 'http://localhost:5501/index.html',
      response_type: 'code',
      userStore: new Oidc.WebStorageStateStore({ store: window.localStorage }),
      scope: 'openid',
      // prompt: 'none'
    };
```

### 已经实现的功能
- **用户信息管理(个人,client,授权)**
- **个人访问token**
- **双重验证**
- **外部登录(discord,微博,支付宝,github)**
- **同意屏幕**
- **邮件**

### Roadmap 
- [ ] Provide startup script and Admin account data
- [ ] Using vue3 rebuild UI
- [ ] Complete the Admin-Dashboard‘s remaining page
- [ ] Admin-Dashboard as a option
- [ ] Multi-tenant support like SAAS
