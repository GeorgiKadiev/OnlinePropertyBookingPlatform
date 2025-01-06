import { createSlice } from '@reduxjs/toolkit';


const roleSlice = createSlice({
  name: 'userRole',
  initialState: null, 
  reducers: {
    setRole: (state, action) => action.payload, 
    clearRole: () => null,
  },
});

export const { setRole, clearRole } = roleSlice.actions; // Export actions
export default roleSlice.reducer; // Export reducer
