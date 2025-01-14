import { createSlice } from "@reduxjs/toolkit";

const initialState = {
  amenities: [],
};

const filterSlice = createSlice({
  name: "filter",
  initialState,
  reducers: {
    setFilters(state, action) {
      state.amenities = action.payload;
    },
  },
});

export const { setFilters } = filterSlice.actions;
export default filterSlice.reducer;
