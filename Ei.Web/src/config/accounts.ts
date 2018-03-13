// import {
//   initState,
//   RegisterProfile,
//   State as AccountState,
//   User as UserModel
// } from 'apollo-authentication-semantic-ui';

// declare global {
//   namespace App { export type User = ClientUserModel; }
// }

// export const profileData = `
//   name
// `;

// export class ClientUserModel extends UserModel {
//   context?: App.Context;

//   constructor(data: UserModel | App.User, context: App.Context) {
//     super(data);
//     this.context = context;
//   }
// }

// let current: AccountState<App.User, RegisterProfile>;

// export function accounts(context: App.Context, cache = true) {
//   if (!current || !cache) {
//     current = initState(
//       context.client,
//       data => new ClientUserModel(data, context),
//       new RegisterProfile(),
//       false
//     ) as any;
//     current.setProfileData(profileData);
//   }
//   return current;
// }
