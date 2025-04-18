export enum ServerStates {
    INIT = 0,
    OK = 1,
    CREATED = 2,
    UPDATED = 3,
    DELETED = 4,
    NOT_FOUND = 5,
    CONFLICT = 6,
    ERROR = 7,
    REBUUILD = 8,
    LOADING = 9
  };

export interface ServerState {
    state: ServerStates;
    message: string;
    hasData: boolean;
}
