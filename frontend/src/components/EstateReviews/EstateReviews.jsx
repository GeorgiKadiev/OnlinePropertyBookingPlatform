import React, { useState, useEffect } from "react";
import { useParams } from "react-router-dom";
import {
  Box,
  Typography,
  Rating,
  CircularProgress,
  List,
  ListItem,
  ListItemText,
} from "@mui/material";
import "./EstateReviews.css";

export default function Reviews() {
  const { id } = useParams(); // Get the property ID from URL parameter

  const [reviews, setReviews] = useState([]); // State to store reviews
  const [loading, setLoading] = useState(true); // State to handle loading state
  const [error, setError] = useState(null); // State to handle errors during fetching

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
        setLoading(false);
      } catch (err) {
        setError(err.message);
        setLoading(false);
      }
    };

    fetchReviews();
  }, [id]);

  if (loading) {
    return (
      <Box className="reviews-loading">
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Box className="reviews-error">
        <Typography color="error">Error: {error}</Typography>
      </Box>
    );
  }

  return (
    <Box className="reviews-container">
      <Typography variant="h5" className="reviews-title">
        Reviews
      </Typography>
      {reviews.length > 0 ? (
        <List className="reviews-list">
          {reviews.map((review) => (
            <Box className="review-item-container" key={review.id}>
              <ListItem className="review-item">
                <ListItemText secondary={review.comment} />
              </ListItem>
              <Box sx={{display: "flex", flexDirection: "column"}}>
              <Box sx={{ "& > legend": { mt: 2 } }} className="review-rating">
                <Typography component="legend">Rating</Typography>
                <Rating name="read-only" value={review.rating} readOnly />
              </Box>
              <ListItem>
                <ListItemText secondary={review.date} />
              </ListItem>
              </Box>
            </Box>
          ))}
        </List>
      ) : (
        <Box className="no-reviews-container">
          <Typography variant="h6" className="no-reviews-message">
            No reviews available for this property.
          </Typography>
        </Box>
      )}
    </Box>
  );
}
