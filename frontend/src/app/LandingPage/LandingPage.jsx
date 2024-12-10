import * as React from "react";
import { Paper, InputBase, IconButton, Box } from "@mui/material";
import SearchIcon from "@mui/icons-material/Search";
import { DemoContainer } from "@mui/x-date-pickers/internals/demo";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import "./LandingPage.css";
import CarouselProperties from "../../components/Carousel/Carousel"

export default function LandingPage() {
  return (
    <div>
    <div className="title-search">
      <h2>Book your stay now</h2>
      <Box
        component="form"
        sx={{ p: "2px 4px", display: "flex", alignItems: "center" }}
      >
        <InputBase
          variant="outlined"
          sx={{ ml: 1, flex: 1 }}
          placeholder="Search for location"
          inputProps={{ "aria-label": "search for location" }}
        />
        <LocalizationProvider dateAdapter={AdapterDayjs}>
          <DemoContainer components={["DatePicker"]}>
            <DatePicker label="Start date" />
            <DatePicker label="End date" />
          </DemoContainer>
        </LocalizationProvider>
        <IconButton
          variant="outlined"
          type="button"
          sx={{ p: "10px" }}
          aria-label="search"
        >
          <SearchIcon />
        </IconButton>
      </Box>
    </div>
    <CarouselProperties/>
    </div>
  );
}
