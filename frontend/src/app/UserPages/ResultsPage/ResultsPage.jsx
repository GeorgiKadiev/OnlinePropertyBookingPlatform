import React, { useState, useEffect } from "react";
import { useSelector } from "react-redux";
import { useNavigate, useLocation } from "react-router-dom";
import {
  Box,
  InputBase,
  IconButton,
  Paper,
  Stack,
  Typography,
  Button,
} from "@mui/material";
import SearchIcon from "@mui/icons-material/Search";
import FiltersSideBar from "../../../components/FiltersSideBar/FiltersSideBar";
import NavBar from "../../../components/NavBar/NavBar";
import "./ResultsPage.css";

export default function ResultsPage() {
  const navigate = useNavigate();
  const location = useLocation();
  const [filters, setFilters] = useState({});
  const [locationEstate, setLocationEstate] = useState("");
  const [propertyData, setPropertyData] = useState([]);
  const [error, setError] = useState(null);

  // Navigate to property details page
  const handleMoreInfo = (id) => {
    navigate(`/property/${id}`);
  };

  // Handle Search
  const handleSearch = async (e) => {
    e.preventDefault();

    if (locationEstate.trim()) {
      const searchFilters = { location: locationEstate };

      try {
        const response = await fetch(
          "http://localhost:5076/api/estate/filter",
          {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
            },
            body: JSON.stringify(searchFilters),
          }
        );

        if (!response.ok) {
          throw new Error("Failed to fetch estates");
        }

        const data = await response.json();
        setPropertyData(data);
        setError(null);
      } catch (err) {
        setError(err.message);
        setPropertyData([]);
      }
    } else {
      // alert("Please enter a location to search.");
    }
  };

  useEffect(() => {
    const queryParams = new URLSearchParams(location.search);

    const initialFilters = {
      location: queryParams.get("location") || "",
      minPrice: queryParams.get("startDate")
        ? parseFloat(queryParams.get("startDate"))
        : null,
      maxPrice: queryParams.get("endDate")
        ? parseFloat(queryParams.get("endDate"))
        : null,
      numberOfPersons: queryParams.get("numberOfPeople")
        ? parseInt(queryParams.get("numberOfPeople"))
        : null,
    };

    setFilters(initialFilters);

    const fetchEstates = async () => {
      try {
        const response = await fetch(
          "http://localhost:5076/api/estate/filter",
          {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
            },
            body: JSON.stringify(initialFilters),
          }
        );

        if (!response.ok) {
          throw new Error("Failed to fetch estates");
        }

        const data = await response.json();
        setPropertyData(data);
        setError(null);
      } catch (err) {
        setError(err.message);
        setPropertyData([]);
      }
    };

    fetchEstates();
  }, [location.search]);

  if (error) {
    return <div>Error: {error}</div>;
  }

  return (
    <div>
      <NavBar />
      <Box className="sidebar-results">
        {/* Sidebar Section */}
        <FiltersSideBar />

        {/* Main Results Section */}
        <div className="main-info">
          <Box className="results">
            <Box
              component="form"
              className="search-bar"
              onSubmit={handleSearch}
              sx={{
                p: "2px 4px",
                display: "flex",
                alignItems: "center",
              }}
            >
              <InputBase
                sx={{ ml: 1, flex: 1 }}
                placeholder="Search for location"
                inputProps={{ "aria-label": "search for location" }}
                value={locationEstate}
                onChange={(e) => setLocationEstate(e.target.value)}
              />
              <IconButton type="submit" sx={{ p: "10px" }} aria-label="search">
                <SearchIcon />
              </IconButton>
            </Box>

            {/* Property List */}
            <Stack className="properties-list" spacing={2}>
              {propertyData.length > 0 ? (
                propertyData.map((property) => (
                  <Paper
                    key={property.id}
                    className="property-item"
                    sx={{
                      display: "flex",
                      alignItems: "center",
                      gap: 2,
                      padding: 2,
                    }}
                  >
                    <img
                      src={property.image}
                      alt={property.title}
                      style={{
                        width: "150px",
                        height: "100px",
                        objectFit: "cover",
                        borderRadius: "8px",
                      }}
                    />
                    <Box sx={{ flex: 1 }}>
                      <Typography variant="h6">{property.title}</Typography>
                      <Typography variant="body2" color="text.secondary">
                        {property.description}
                      </Typography>
                      <Typography variant="body1" color="text.secondary">
                        ${property.pricePerNight} per night
                      </Typography>
                    </Box>
                    <Button
                      variant="contained"
                      color="primary"
                      onClick={() => handleMoreInfo(property.id)}
                    >
                      More Info
                    </Button>
                  </Paper>
                ))
              ) : (
                <Typography>
                  No properties found for the given filters.
                </Typography>
              )}
            </Stack>
          </Box>
        </div>
      </Box>
    </div>
  );
}
