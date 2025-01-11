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

  const token = useSelector((state) => state.token); // Get token from Redux
  const userId = useSelector((state) => state.id); // Get token from Redux
  const [filters, setFilters] = useState({});

  const [propertyData, setPropertyData] = useState([]); // State for holding fetched data
  const [loading, setLoading] = useState(true); // State for loading state

  const handleMoreInfo = (id) => {
    console.log(id);
    navigate(`/property/${id}`);
  };

  useEffect(() => {
    // Extract query params from URL
    const queryParams = new URLSearchParams(location.search);

    const filters = {
      location: queryParams.get("location"),
      minPrice: parseFloat(queryParams.get("startDate")) || null,
      maxPrice: parseFloat(queryParams.get("endDate")) || null,
      numberOfPersons: parseInt(queryParams.get("numberOfPeople")) || null,
    };

    setFilters(filters);
    console.log(filters);

    const fetchEstates = async () => {
      try {
        const response = await fetch(
          "http://localhost:5076/api/estate/filter",
          {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
            },
            body: JSON.stringify(filters),
          }
        );

        if (!response.ok) {
          throw new Error("Failed to fetch estates");
        }

        const data = await response.json();
        console.log(data);
        setPropertyData(data); // Store fetched data in state
        setLoading(false); // Set loading to false once data is fetched
      } catch (error) {
        console.error("Error fetching estates:", error);
        setLoading(false); // Set loading to false if there's an error
      }
    };

    fetchEstates();
  }, [userId, token]);

  if (loading) {
    return <div>Loading...</div>; // Show loading text until data is fetched
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
            {/* Search Bar */}
            <Box
              component="form"
              className="search-bar"
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
              />
              <IconButton type="button" sx={{ p: "10px" }} aria-label="search">
                <SearchIcon />
              </IconButton>
            </Box>

            {/* Property List */}
            <Stack className="properties-list" spacing={2}>
              {propertyData.map((property) => (
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
                  {/* Property Image */}
                  <img
                    src={property.image}
                    // alt={property.title}
                    style={{
                      width: "150px",
                      height: "100px",
                      objectFit: "cover",
                      borderRadius: "8px",
                    }}
                  />
                  {/* Property Details */}
                  <Box sx={{ flex: 1 }}>
                    <Typography variant="h6">{property.title}</Typography>
                    <Typography variant="h10">
                      {property.description}
                    </Typography>
                    <Typography variant="body1" color="text.secondary">
                      {property.pricePerNight}
                    </Typography>
                  </Box>
                  {/* More Info Button */}
                  <Button
                    variant="contained"
                    color="primary"
                    onClick={() => handleMoreInfo(property.id)}
                  >
                    More Info
                  </Button>
                </Paper>
              ))}
            </Stack>
          </Box>
        </div>
      </Box>
    </div>
  );
}
