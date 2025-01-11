import React, { useState, useEffect } from "react";
import { useParams } from "react-router-dom";
import {
  Box,
  Typography,
  Button,
  Grid,
  Paper,
  CircularProgress,
} from "@mui/material";
import Reviews from "../../../components/EstateReviews/EstateReviews";
import NavBar from "../../../components/NavBar/NavBar";
import "./PropertyPage.css";

export default function PropertyPage() {
  const { id } = useParams(); // Get the property ID from URL parameter
  const [property, setProperty] = useState(null); // State to store property data
  const [loading, setLoading] = useState(true); // State to handle loading state
  const [error, setError] = useState(null); // State to handle errors during fetching

  useEffect(() => {
    const fetchProperty = async () => {
      try {
        const response = await fetch(`http://localhost:5076/api/estate/${id}`);
        if (!response.ok) {
          throw new Error("Property not found");
        }
        const data = await response.json();
        setProperty(data);
        setLoading(false);
      } catch (err) {
        setError(err.message);
        setLoading(false);
      }
    };

    fetchProperty();
  }, [id]);

  if (loading) {
    return (
      <Box className="loading-container">
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Box className="error-container">
        <Typography variant="h6" color="error">
          Error: {error}
        </Typography>
      </Box>
    );
  }

  return (
    <div>
      <NavBar/>
      <Box className="property-page">
        {/* Title */}
        <Typography variant="h4" className="property-title">
          {property.title}
        </Typography>

        {/* Image Section */}
        <Grid container spacing={2} className="property-images">
          {property.images &&
            property.images.map((image, index) => (
              <Grid item xs={12} sm={6} md={4} key={index}>
                <Paper elevation={3} className="image-paper">
                  <img
                    src={image}
                    alt={`Property image ${index + 1}`}
                    className="property-image"
                  />
                </Paper>
              </Grid>
            ))}
        </Grid>

        {/* Description and Booking */}
        <Box className="property-details">
          <Box className="property-info">
            <Typography variant="body1" className="property-description">
              {property.description}
            </Typography>
            <Typography variant="h6" className="property-price">
              {property.price}
            </Typography>
          </Box>
          <Button
            variant="contained"
            color="primary"
            size="large"
            onClick={() => alert(`Booking Property ${id}`)}
            className="booking-button"
          >
            Book
          </Button>
        </Box>

        {/* Reviews Section */}
        <Reviews estateId={id} />
      </Box>
    </div>
  );
}
