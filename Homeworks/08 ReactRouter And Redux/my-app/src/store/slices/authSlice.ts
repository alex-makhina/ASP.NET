import { createSlice, type PayloadAction } from "@reduxjs/toolkit";

interface AuthState {
  user: string | null;
  isAuthenticated: boolean;
}

interface AuthPayload {
  email: string;
  password: string;
}

const initialState: AuthState = {
  user: null,
  isAuthenticated: false
}

const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    logout: (state) => {
      state.user = null;
      state.isAuthenticated = false;
    },
    login: (state, action: PayloadAction<AuthPayload>) => {
      state.user = action.payload.email;
      state.isAuthenticated = true;
    },
    register: (state, action: PayloadAction<AuthPayload>) => {
      state.user = action.payload.email;
      state.isAuthenticated = true;
    }
  }
});

export const { logout, login, register } = authSlice.actions;
export default authSlice.reducer;
