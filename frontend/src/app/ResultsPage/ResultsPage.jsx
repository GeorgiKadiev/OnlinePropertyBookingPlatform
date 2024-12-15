// import React, { useState } from "react";
import { Box, InputBase, IconButton, Paper, Stack } from "@mui/material";
import SearchIcon from "@mui/icons-material/Search";
import "./ResultsPage.css";
import FiltersSideBar from "../../components/FiltersSideBar/FiltersSideBar";

export default function ResultsPage() {
  return (
    <Box className="sidebar-results">
      {/* Sidebar Section */}
      <FiltersSideBar />

      {/* Main Results Section */}
      <div className="main-info">
        <Box className="results">
          <Box
            component="form"
            className="search-bar"
            sx={{
              p: "2px 4px",
              display: "flex",
              alignItems: "center",
            //   maxWidth: "500px", // Optional: Limit the search bar width
            }}
          >
            <InputBase
              sx={{ ml: 1, flex: 1 }}
              placeholder="Search for location"
              inputProps={{ "aria-label": "search for location" }}
            />
            <IconButton type="button" sx={{ p: "10px" }} aria-label="search">
              <SearchIcon />
            </IconButton>
          </Box>
          <Stack className="propetries-list" spacing={2}>
            <Paper>Item 1</Paper>
            <Paper>Item 2</Paper>
            <Paper>Item 3</Paper>
          </Stack>
        </Box>
      </div>
    </Box>
  );
}
