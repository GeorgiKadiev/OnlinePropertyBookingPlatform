import React, { useState, useEffect } from "react";
import { useParams } from "react-router-dom";
import {
  Box,
  Typography,
  Button,
  Grid,
  Paper,
  CircularProgress,
  List,
  ListItem,
  ListItemText,
} from "@mui/material";
import "./PropertyPage.css"; // Import the CSS file

export default function PropertyPage() {
  const { id } = useParams(); // Get the property ID from URL parameter
  const [property, setProperty] = useState(null); // State to store property data
  const [reviews, setReviews] = useState([]); // State to store reviews
  const [loading, setLoading] = useState(true); // State to handle loading state
  const [reviewsLoading, setReviewsLoading] = useState(true); // State for review loading
  const [error, setError] = useState(null); // State to handle errors during fetching
  const [reviewsError, setReviewsError] = useState(null); // State to handle review fetch errors

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

  useEffect(() => {
    const fetchReviews = async () => {
      try {
        const response = await fetch(
          `http://localhost:5076/api/review/estate-reviews/${id}`
        );
        if (!response.ok) {
          throw new Error("Failed to fetch reviews");
        }
        const data = await response.json();
        setReviews(data);
        setReviewsLoading(false);
      } catch (err) {
        setReviewsError(err.message);
        setReviewsLoading(false);
      }
    };

    fetchReviews();
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
      <Box className="reviews-section">
        <Typography variant="h5" className="reviews-title">
          Reviews
        </Typography>
        {reviewsLoading ? (
          <CircularProgress />
        ) : reviewsError ? (
          <Typography color="error">Error: {reviewsError}</Typography>
        ) : reviews.length > 0 ? (
          <List className="reviews-list">
            {reviews.map((review) => (
              <ListItem key={review.id} className="review-item">
                <ListItemText
                  primary={`User: ${review.username}`}
                  secondary={review.comment}
                />
              </ListItem>
            ))}
          </List>
        ) : (
          <Typography className="no-reviews">
            No reviews for this property.
          </Typography>
        )}
      </Box>
    </Box>
  );
}
